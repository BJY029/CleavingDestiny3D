// Deployed from unity-character-onboarding/scripts. Editor-only; no live UGS writes.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public static class CharacterOnboarding
{
    public const string Template = "Assets/Resources/Player/Player1.prefab";
    public const string Catalog = "Assets/4_ScriptableObjects/ShopItems/CharacterSkins/CharacterSkinCatalogSO.asset";
    private const string Production = "Assets/CharacterOnboarding";
    private const string Verification = "Assets/CharacterOnboardingVerification";

    [Serializable]
    public class Report
    {
        public string id, displayName, sourceHash, folder, modelPath, prefabPath, skinPath, iconPath, previewHash, error;
        public int price;
        public bool verificationOnly, validated, registered;
        public List<string> warnings = new List<string>();
        public List<string> checks = new List<string>();
        public List<string> previews = new List<string>();
    }

    public static string Build(string fbxPath, string displayName, int price, string skinId = null, bool verificationOnly = false)
    {
        if (AnimationMode.InAnimationMode()) throw new InvalidOperationException("Exit animation preview mode before onboarding.");
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit play mode before onboarding.");
        string source = Path.GetFullPath(fbxPath);
        if (!File.Exists(source) || !string.Equals(Path.GetExtension(source), ".fbx", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("An existing FBX file is required.");
        if (string.IsNullOrWhiteSpace(displayName) || price < 0) throw new ArgumentException("Name and nonnegative integer coin price required.");
        string id = skinId ?? Regex.Replace(displayName.ToLowerInvariant(), "[^a-z0-9]+", "_").Trim('_');
        if (!Regex.IsMatch(id, "^[a-z][a-z0-9_]{0,63}$")) throw new ArgumentException("Supply a stable ASCII skinId for this name (lowercase letters, digits, underscores; starts with a letter).");
        string folder = (verificationOnly ? Verification : Production) + "/" + id;
        string prefab = verificationOnly ? folder + "/Resources/Player/Characters/" + id + ".prefab" : "Assets/Resources/Player/Characters/" + id + ".prefab";
        string hash;
        using (var sha = SHA256.Create()) using (var stream = File.OpenRead(source)) hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
        foreach (string texture in TextureFiles(source).OrderBy(p => p, StringComparer.Ordinal))
            using (var sha = SHA256.Create()) hash += ":" + Path.GetFileName(texture) + ":" + BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(texture)));
        string sourceAsset = ToAssetPath(source);
        if (!string.IsNullOrEmpty(sourceAsset)) hash += ":" + AssetDatabase.GetAssetDependencyHash(sourceAsset);
        string manifest = folder + "/report.json";
        var catalog = AssetDatabase.LoadAssetAtPath<CharacterSkinCatalogSO>(Catalog);
        if (catalog == null) throw new InvalidOperationException("Project character catalog missing.");
        Report report;
        if (Directory.Exists(folder))
        {
            if (!File.Exists(manifest)) throw new InvalidOperationException("Output folder is not owned by this tool.");
            report = JsonUtility.FromJson<Report>(File.ReadAllText(manifest));
            if (report == null || report.id != id || report.sourceHash != hash || report.displayName != displayName || report.price != price || report.verificationOnly != verificationOnly || report.prefabPath != prefab)
                throw new InvalidOperationException("Existing output belongs to another input. Use a different ID; do not overwrite.");
            if (report.validated) { ValidatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefab)); return JsonUtility.ToJson(report, true); }
        }
        else
        {
            if (catalog.TryGetSkin(id, out _) || AssetDatabase.LoadAssetAtPath<GameObject>(prefab) != null)
                throw new InvalidOperationException("Skin ID or prefab already exists.");
            Directory.CreateDirectory(folder);
            report = new Report { id = id, displayName = displayName, price = price, sourceHash = hash, folder = folder,
                modelPath = folder + "/Model.fbx", prefabPath = prefab, skinPath = folder + "/Skin.asset", iconPath = folder + "/Icon.png", verificationOnly = verificationOnly };
            Save(report);
        }
        report.validated = false;
        report.error = null;
        report.checks.Clear(); report.warnings.Clear(); report.previews.Clear();
        try
        {
            Directory.CreateDirectory(folder + "/Textures"); Directory.CreateDirectory(folder + "/Materials");
            Directory.CreateDirectory(Path.GetDirectoryName(prefab));
            if (Path.GetFullPath(report.modelPath) != source) File.Copy(source, report.modelPath, true);
            CopyExternalTextures(source, folder + "/Textures");
            AssetDatabase.Refresh();
            var importer = (ModelImporter)AssetImporter.GetAtPath(report.modelPath);
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            var original = AssetImporter.GetAtPath(ToAssetPath(source)) as ModelImporter;
            var humanDescription = original != null && original.animationType == ModelImporterAnimationType.Human ? original.humanDescription : importer.humanDescription;
            humanDescription.skeleton = Array.Empty<SkeletonBone>();
            importer.humanDescription = humanDescription;
            importer.SaveAndReimport();
            var rigCheck = AssetDatabase.LoadAssetAtPath<GameObject>(report.modelPath).GetComponent<Animator>();
            if (rigCheck == null || rigCheck.avatar == null || !rigCheck.avatar.isValid || !rigCheck.avatar.isHuman)
                throw new InvalidOperationException("Humanoid mapping failed. A rigged FBX with mapped required bones is needed; automatic rigging is not supported.");
            importer = (ModelImporter)AssetImporter.GetAtPath(report.modelPath);
            importer.ExtractTextures(folder + "/Textures");
            AssetDatabase.Refresh();
            NormalizeImportScale(report);
            PrepareMaterials(report, original);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(report.modelPath);
            var avatar = model == null ? null : model.GetComponent<Animator>();
            if (avatar == null || avatar.avatar == null || !avatar.avatar.isValid || !avatar.avatar.isHuman)
                throw new InvalidOperationException("Humanoid mapping failed. Supply a rigged FBX with valid required bones; no automatic rigging is performed.");
            report.checks.Add("Humanoid avatar valid");
            BuildPrefab(report, model);
            ValidatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefab));
            report.checks.Add("Player, Photon, animation, socket, effect and body references valid");
            RenderPreviews(report);
            AssetDatabase.ImportAsset(report.iconPath, ImportAssetOptions.ForceSynchronousImport);
            var iconImporter = (TextureImporter)AssetImporter.GetAtPath(report.iconPath);
            iconImporter.textureType = TextureImporterType.Sprite;
            iconImporter.alphaIsTransparency = true;
            iconImporter.SaveAndReimport();
            var skin = AssetDatabase.LoadAssetAtPath<CharacterSkinSO>(report.skinPath);
            if (skin == null) { skin = ScriptableObject.CreateInstance<CharacterSkinSO>(); AssetDatabase.CreateAsset(skin, report.skinPath); }
            Set(skin, "skinId", id); Set(skin, "displayName", displayName); Set(skin, "price", price);
            Set(skin, "icon", AssetDatabase.LoadAssetAtPath<Sprite>(report.iconPath));
            Set(skin, "characterPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(prefab));
            string photonPath = "Player/Characters/" + id;
            Set(skin, "photonPrefabPath", photonPath);
            string resolved;
            if (!CharacterSpawnResolver.TryGetPrefabPath(skin, out resolved) || resolved != photonPath) throw new InvalidOperationException("Photon resource mapping invalid.");
            File.WriteAllText(folder + "/remote-config-entry.json", "{\n  \"" + id + "\": { \"currency\": \"coins\", \"price\": " + price + " }\n}\n");
            report.previewHash = AssetDatabase.GetAssetDependencyHash(report.prefabPath).ToString();
            report.validated = true;
            report.checks.Add("Resources loading and skin metadata valid; no catalog mutation yet");
            AssetDatabase.SaveAssetIfDirty(skin); Save(report); AssetDatabase.Refresh();
            return JsonUtility.ToJson(report, true);
        }
        catch (Exception error) { report.error = error.Message; Save(report); throw; }
    }

    public static string RefreshPreviews(string reportPath)
    {
        var r = LoadOwnedReport(reportPath); r.validated = false; r.error = null; r.previews.Clear(); Save(r);
        try
        {
            ValidatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(r.prefabPath)); RenderPreviews(r);
            AssetDatabase.ImportAsset(r.iconPath, ImportAssetOptions.ForceSynchronousImport);
            r.previewHash = AssetDatabase.GetAssetDependencyHash(r.prefabPath).ToString(); r.validated = true; Save(r);
            return JsonUtility.ToJson(r, true);
        }
        catch (Exception error) { r.error = error.Message; Save(r); throw; }
    }

    public static string Register(string reportPath, bool visualReviewPassed)
    {
        var r = LoadOwnedReport(reportPath);
        if (!r.validated || !visualReviewPassed) throw new InvalidOperationException("Successful build and actual visual review are required before registration.");
        if (r.verificationOnly) throw new InvalidOperationException("Verification candidates cannot enter the production catalog.");
        ValidatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(r.prefabPath));
        if (r.previewHash != AssetDatabase.GetAssetDependencyHash(r.prefabPath).ToString()) throw new InvalidOperationException("Prefab dependencies changed. Refresh previews and repeat visual review.");
        var skin = AssetDatabase.LoadAssetAtPath<CharacterSkinSO>(r.skinPath);
        string path;
        if (skin == null || skin.SkinId != r.id || skin.Price != r.price || !CharacterSpawnResolver.TryGetPrefabPath(skin, out path))
            throw new InvalidOperationException("Candidate changed after validation.");
        var catalog = AssetDatabase.LoadAssetAtPath<CharacterSkinCatalogSO>(Catalog);
        CharacterSkinSO existing;
        if (catalog.TryGetSkin(r.id, out existing))
        {
            if (existing != skin) throw new InvalidOperationException("ID already belongs to another skin.");
        }
        else
        {
            var so = new SerializedObject(catalog); var list = so.FindProperty("skins");
            int index = list.arraySize; list.InsertArrayElementAtIndex(index); list.GetArrayElementAtIndex(index).objectReferenceValue = skin;
            so.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(catalog); AssetDatabase.SaveAssetIfDirty(catalog);
        }
        r.registered = true; Save(r);
        return JsonUtility.ToJson(r, true);
    }

    private static void Save(Report r) { File.WriteAllText(r.folder + "/report.json", JsonUtility.ToJson(r, true)); }
    private static Report LoadOwnedReport(string path)
    {
        string full = Path.GetFullPath(path), basePath = Path.GetFullPath(Production) + Path.DirectorySeparatorChar, testPath = Path.GetFullPath(Verification) + Path.DirectorySeparatorChar;
        if ((!full.StartsWith(basePath, StringComparison.OrdinalIgnoreCase) && !full.StartsWith(testPath, StringComparison.OrdinalIgnoreCase)) || Path.GetFileName(path) != "report.json")
            throw new ArgumentException("Expected a generated onboarding report.");
        var r = JsonUtility.FromJson<Report>(File.ReadAllText(full));
        if (r == null || Path.GetFullPath(r.folder + "/report.json") != full || !Regex.IsMatch(r.id, "^[a-z][a-z0-9_]{0,63}$"))
            throw new InvalidOperationException("Invalid generated report.");
        string expectedFolder = (r.verificationOnly ? Verification : Production) + "/" + r.id;
        if (r.folder != expectedFolder || r.skinPath != r.folder + "/Skin.asset" || r.iconPath != r.folder + "/Icon.png" || r.prefabPath != (r.verificationOnly ? r.folder + "/Resources/Player/Characters/" + r.id + ".prefab" : "Assets/Resources/Player/Characters/" + r.id + ".prefab")) throw new InvalidOperationException("Generated report paths do not match its ID.");
        return r;
    }
    private static string ToAssetPath(string absolute)
    {
        string project = Path.GetFullPath(".") + Path.DirectorySeparatorChar;
        return absolute.StartsWith(project, StringComparison.OrdinalIgnoreCase) ? absolute.Substring(project.Length).Replace('\\', '/') : "";
    }
    private static void Set(Object target, string name, object value)
    {
        var so = new SerializedObject(target); var p = so.FindProperty(name);
        if (p == null) throw new InvalidOperationException("Template schema changed: " + name);
        if (value is int) p.intValue = (int)value;
        else if (value is string) p.stringValue = (string)value;
        else p.objectReferenceValue = value as Object;
        so.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(target);
    }
    private static Transform Ref(Component component, string field) { return new SerializedObject(component).FindProperty(field).objectReferenceValue as Transform; }
    private static List<string> TextureFiles(string source)
    {
        var extensions = new HashSet<string>(new[] { ".png", ".jpg", ".jpeg", ".tga", ".tif", ".tiff", ".psd", ".exr", ".bmp" });
        var candidates = Directory.GetFiles(Path.GetDirectoryName(source)).Where(p => extensions.Contains(Path.GetExtension(p).ToLowerInvariant())).ToList();
        string fbm = Path.Combine(Path.GetDirectoryName(source), Path.GetFileNameWithoutExtension(source) + ".fbm");
        if (Directory.Exists(fbm)) candidates.AddRange(Directory.GetFiles(fbm, "*", SearchOption.AllDirectories).Where(p => extensions.Contains(Path.GetExtension(p).ToLowerInvariant())));
        return candidates;
    }
    private static void CopyExternalTextures(string source, string target)
    {
        foreach (string p in TextureFiles(source))
        {
            string destination = Path.Combine(target, Path.GetFileName(p));
            if (File.Exists(destination) && !File.ReadAllBytes(destination).SequenceEqual(File.ReadAllBytes(p))) throw new InvalidOperationException("Texture name collision: " + p);
            if (!File.Exists(destination)) File.Copy(p, destination);
        }
    }

    private static void PrepareMaterials(Report r, ModelImporter original)
    {
        var importer = (ModelImporter)AssetImporter.GetAtPath(r.modelPath);
        var materials = AssetDatabase.LoadAllAssetsAtPath(r.modelPath).OfType<Material>().ToDictionary(m => new AssetImporter.SourceAssetIdentifier(m), m => m);
        foreach (var entry in importer.GetExternalObjectMap())
            if (entry.Value is Material && !materials.ContainsKey(entry.Key)) materials.Add(entry.Key, (Material)entry.Value);
        var originals = original == null ? new Dictionary<AssetImporter.SourceAssetIdentifier, Object>() : original.GetExternalObjectMap();
        int i = 0;
        foreach (var entry in originals)
            if (entry.Value is Material && !materials.ContainsKey(entry.Key)) materials.Add(entry.Key, (Material)entry.Value);
        foreach (var entry in materials.OrderBy(e => e.Value.name, StringComparer.Ordinal))
        {
            Material embedded = entry.Value;
            Material source = embedded;
            var key = entry.Key;
            Object remapped;
            if (originals.TryGetValue(key, out remapped) && remapped is Material) source = (Material)remapped;
            string path = r.folder + "/Materials/" + i++ + "_" + Regex.Replace(embedded.name, "[^a-zA-Z0-9_-]", "_") + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                if (source == embedded && AssetDatabase.IsSubAsset(source) && AssetDatabase.GetAssetPath(source) == r.modelPath)
                {
                    string error = AssetDatabase.ExtractAsset(embedded, path);
                    if (!string.IsNullOrEmpty(error)) throw new InvalidOperationException(error);
                    material = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
                else { material = new Material(source); AssetDatabase.CreateAsset(material, path); }
            }
            Texture color = source.HasProperty("_BaseMap") ? source.GetTexture("_BaseMap") : source.mainTexture;
            Texture normal = source.HasProperty("_BumpMap") ? source.GetTexture("_BumpMap") : null;
            bool transparent = source.renderQueue >= (int)RenderQueue.Transparent || (source.HasProperty("_Surface") && source.GetFloat("_Surface") > 0) || (source.HasProperty("_Mode") && source.GetFloat("_Mode") >= 2);
            bool cutout = source.IsKeywordEnabled("_ALPHATEST_ON") || (source.HasProperty("_AlphaClip") && source.GetFloat("_AlphaClip") > 0) || (source.HasProperty("_Mode") && source.GetFloat("_Mode") == 1);
            float cutoff = source.HasProperty("_Cutoff") ? source.GetFloat("_Cutoff") : 0.5f;
            float metallic = source.HasProperty("_Metallic") ? source.GetFloat("_Metallic") : 0;
            float smoothness = source.HasProperty("_Smoothness") ? source.GetFloat("_Smoothness") : source.HasProperty("_Glossiness") ? source.GetFloat("_Glossiness") : 0.5f;
            float cull = source.HasProperty("_Cull") ? source.GetFloat("_Cull") : (float)CullMode.Back;
            Texture metalMap = source.HasProperty("_MetallicGlossMap") ? source.GetTexture("_MetallicGlossMap") : null;
            Texture emission = source.HasProperty("_EmissionMap") ? source.GetTexture("_EmissionMap") : null;
            Color emissionColor = source.HasProperty("_EmissionColor") ? source.GetColor("_EmissionColor") : Color.black;
            Vector2 uvScale = source.mainTextureScale, uvOffset = source.mainTextureOffset;
            Color tint = source.HasProperty("_BaseColor") ? source.GetColor("_BaseColor") : source.HasProperty("_Color") ? source.GetColor("_Color") : Color.white;
            if (GraphicsSettings.currentRenderPipeline != null && GraphicsSettings.currentRenderPipeline.GetType().Name.Contains("Universal"))
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
                material.shader = shader; material.SetColor("_BaseColor", tint);
                material.SetTexture("_BaseMap", CopyTexture(color, r.folder));
                material.SetTexture("_BumpMap", CopyTexture(normal, r.folder, true));
                material.SetTextureScale("_BaseMap", uvScale); material.SetTextureOffset("_BaseMap", uvOffset);
                material.SetFloat("_Metallic", metallic); material.SetFloat("_Smoothness", smoothness); material.SetFloat("_Cull", cull);
                material.SetTexture("_MetallicGlossMap", CopyTexture(metalMap, r.folder));
                if (metalMap != null) material.EnableKeyword("_METALLICSPECGLOSSMAP");
                material.SetTexture("_EmissionMap", CopyTexture(emission, r.folder)); material.SetColor("_EmissionColor", emissionColor);
                if (emission != null || emissionColor.maxColorComponent > 0) material.EnableKeyword("_EMISSION");
                material.SetFloat("_AlphaClip", cutout ? 1 : 0); material.SetFloat("_Cutoff", cutoff);
                if (cutout) material.EnableKeyword("_ALPHATEST_ON");
                material.SetFloat("_Surface", transparent ? 1 : 0);
                if (transparent)
                {
                    material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha); material.SetFloat("_ZWrite", 0);
                    material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); material.SetOverrideTag("RenderType", "Transparent"); material.renderQueue = (int)RenderQueue.Transparent;
                }
                else
                {
                    material.SetFloat("_SrcBlend", (float)BlendMode.One); material.SetFloat("_DstBlend", (float)BlendMode.Zero); material.SetFloat("_ZWrite", 1);
                    material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT"); material.SetOverrideTag("RenderType", cutout ? "TransparentCutout" : "Opaque"); material.renderQueue = cutout ? (int)RenderQueue.AlphaTest : (int)RenderQueue.Geometry;
                }
                if (normal != null) material.EnableKeyword("_NORMALMAP");
            }
            if (material.mainTexture == null) r.warnings.Add("Material '" + material.name + "' has no base texture. Confirm that flat color is intentional; supply missing textures before registration.");
            EditorUtility.SetDirty(material); AssetDatabase.SaveAssetIfDirty(material); importer.AddRemap(key, material);
        }
        if (materials.Count == 0) throw new InvalidOperationException("No imported materials found. Resolve importer material settings before retrying.");
        importer.SaveAndReimport();
    }
    private static Texture CopyTexture(Texture texture, string folder, bool normal = false)
    {
        if (texture == null) return null;
        string source = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(source)) throw new InvalidOperationException("Texture is not a persistent asset: " + texture.name);
        string path = folder + "/Textures/" + Path.GetFileName(source);
        if (source != path && !File.Exists(path) && !AssetDatabase.CopyAsset(source, path)) throw new InvalidOperationException("Texture copy failed: " + source);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null && normal && ti.textureType != TextureImporterType.NormalMap) { ti.textureType = TextureImporterType.NormalMap; ti.SaveAndReimport(); }
        return AssetDatabase.LoadAssetAtPath<Texture>(path);
    }

    // Humanoid retargeting uses Avatar scale. Scaling rig children after Avatar creation breaks that relationship.
    private static void NormalizeImportScale(Report report)
    {
        var template = PrefabUtility.LoadPrefabContents(Template);
        var modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(report.modelPath);
        var instance = Object.Instantiate(modelAsset);
        try
        {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(instance, template.scene);
            float targetHeight = BoundsOf(BodyRoots(template).SelectMany(g => g.GetComponentsInChildren<Renderer>(true))).size.y;
            float sourceHeight = BoundsOf(instance.GetComponentsInChildren<Renderer>(true)).size.y;
            float ratio = targetHeight / sourceHeight;
            if (float.IsNaN(ratio) || float.IsInfinity(ratio) || ratio <= 0) throw new InvalidOperationException("Cannot normalize FBX import scale.");
            if (Mathf.Abs(ratio - 1f) > 0.001f)
            {
                var importer = (ModelImporter)AssetImporter.GetAtPath(report.modelPath);
                importer.globalScale *= ratio;
                var description = importer.humanDescription;
                description.skeleton = Array.Empty<SkeletonBone>();
                importer.humanDescription = description;
                importer.SaveAndReimport();
            }
            report.checks.Add("Model height normalized through FBX import scale; Avatar and bones remain consistent");
        }
        finally { Object.DestroyImmediate(instance); PrefabUtility.UnloadPrefabContents(template); }
    }

    private static void BuildPrefab(Report report, GameObject modelAsset)
    {
        var player = PrefabUtility.LoadPrefabContents(Template);
        GameObject model = null;
        try
        {
            var target = player.GetComponent<Animator>(); target.Rebind(); target.Update(0);
            var oldBodyRoots = BodyRoots(player);
            Bounds baseline = BoundsOf(oldBodyRoots.SelectMany(g => g.GetComponentsInChildren<Renderer>(true)));
            Transform oldHips = target.GetBoneTransform(HumanBodyBones.Hips);
            if (oldHips == null) throw new InvalidOperationException("Template has no humanoid hips.");
            Transform oldRig = oldHips; while (oldRig.parent != player.transform) oldRig = oldRig.parent;
            model = Object.Instantiate(modelAsset); model.name = "ImportedModel";
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(model, player.scene);
            model.transform.SetParent(player.transform, false);
            var source = model.GetComponent<Animator>(); source.Rebind(); source.Update(0);
            Transform right = source.GetBoneTransform(HumanBodyBones.RightHand);
            Transform left = source.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform head = source.GetBoneTransform(HumanBodyBones.Head);
            Transform spine = source.GetBoneTransform(HumanBodyBones.Spine);
            if (right == null || left == null || head == null || spine == null) throw new InvalidOperationException("Required hand/head/spine bones are not mapped.");
            Renderer[] newBodies = model.GetComponentsInChildren<Renderer>(true);
            if (newBodies.Length == 0 || newBodies.Any(r => r.sharedMaterials.Any(m => m == null || m.shader == null || m.shader.name.Contains("InternalError")))) throw new InvalidOperationException("Missing mesh/material/shader.");
            Bounds bounds = BoundsOf(newBodies);
            float scale = baseline.size.y / bounds.size.y;
            if (float.IsNaN(scale) || float.IsInfinity(scale) || scale <= 0) throw new InvalidOperationException("Cannot normalize model height.");
            if (Mathf.Abs(scale - 1f) > 0.01f) throw new InvalidOperationException("FBX import scale normalization failed; refusing to scale humanoid children.");
            report.checks.Add("Humanoid skeleton and meshes retain imported transforms (no post-Avatar scaling)");
            var map = new Dictionary<Transform, Transform>();
            for (int bone = 0; bone < (int)HumanBodyBones.LastBone; bone++)
            {
                var oldBone = target.GetBoneTransform((HumanBodyBones)bone); var newBone = source.GetBoneTransform((HumanBodyBones)bone);
                if (oldBone != null && newBone != null) map[oldBone] = newBone;
            }
            var animation = player.GetComponent<PlayerAnimationController>();
            var axe = Ref(animation, "axeTransform");
            Vector3 oldHandScale = axe.parent.lossyScale;
            axe.SetParent(right, false);
            Vector3 handScale = right.lossyScale;
            axe.localScale = Vector3.Scale(axe.localScale, new Vector3(oldHandScale.x / handScale.x, oldHandScale.y / handScale.y, oldHandScale.z / handScale.z));
            var effects = player.GetComponent<PlayerEffectPoints>();
            Set(effects, "head", head); Set(effects, "body", spine); Set(effects, "rightHand", right); Set(effects, "leftHand", left);
            var pc = new SerializedObject(player.GetComponent<PlayerController>()); var bodyArray = pc.FindProperty("characterModelObjects");
            bodyArray.arraySize = newBodies.Length;
            for (int i = 0; i < newBodies.Length; i++) bodyArray.GetArrayElementAtIndex(i).objectReferenceValue = newBodies[i].gameObject;
            pc.ApplyModifiedPropertiesWithoutUndo();
            target.enabled = false;
            Avatar avatar = source.avatar;
            foreach (Transform t in model.transform.Cast<Transform>().ToArray()) t.SetParent(player.transform, true);
            Object.DestroyImmediate(model); model = null;
            // Repair every surviving serialized transform reference before removing the old rig.
            foreach (var component in player.GetComponentsInChildren<Component>(true))
            {
                if (component == null || component is Transform || component.transform.IsChildOf(oldRig) || oldBodyRoots.Any(g => component.transform == g.transform || component.transform.IsChildOf(g.transform))) continue;
                var so = new SerializedObject(component); var property = so.GetIterator(); bool changed = false;
                while (property.Next(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                    var old = property.objectReferenceValue as Transform;
                    if (old == null || (old != oldRig && !old.IsChildOf(oldRig))) continue;
                    Transform replacement;
                    if (!map.TryGetValue(old, out replacement)) throw new InvalidOperationException("Unmapped gameplay bone reference: " + component.GetType().Name + "." + property.propertyPath);
                    property.objectReferenceValue = replacement; changed = true;
                }
                if (changed) so.ApplyModifiedPropertiesWithoutUndo();
            }
            foreach (var body in oldBodyRoots) if (body != null && body.transform != oldRig && !body.transform.IsChildOf(oldRig)) Object.DestroyImmediate(body);
            Object.DestroyImmediate(oldRig.gameObject);
            target.avatar = avatar; target.enabled = true; target.Rebind(); target.Update(0);
            player.name = report.id;
            ValidatePrefab(player);
            PrefabUtility.SaveAsPrefabAsset(player, report.prefabPath);
        }
        finally { if (model != null) Object.DestroyImmediate(model); PrefabUtility.UnloadPrefabContents(player); }
    }
    private static GameObject[] BodyRoots(GameObject p)
    {
        var array = new SerializedObject(p.GetComponent<PlayerController>()).FindProperty("characterModelObjects");
        var list = new List<GameObject>();
        for (int i = 0; i < array.arraySize; i++) { var value = array.GetArrayElementAtIndex(i).objectReferenceValue as GameObject; if (value != null) list.Add(value); }
        if (list.Count == 0 || list.Any(g => g == p)) throw new InvalidOperationException("Template body roots must explicitly identify body meshes.");
        return list.ToArray();
    }
    private static Bounds BoundsOf(IEnumerable<Renderer> renderers)
    {
        var list = renderers.ToArray(); if (list.Length == 0) throw new InvalidOperationException("No renderers.");
        var bounds = list[0].bounds; foreach (var r in list.Skip(1)) bounds.Encapsulate(r.bounds);
        if (bounds.size.y < 0.001f) throw new InvalidOperationException("Degenerate model bounds.");
        return bounds;
    }
    public static void ValidatePrefab(GameObject p)
    {
        if (!CharacterSpawnResolver.IsValidPlayerPrefab(p)) throw new InvalidOperationException("Player prefab component/avatar validation failed.");
        var a = p.GetComponent<Animator>();
        Transform upper = a.GetBoneTransform(HumanBodyBones.LeftUpperLeg), lower = a.GetBoneTransform(HumanBodyBones.LeftLowerLeg), foot = a.GetBoneTransform(HumanBodyBones.LeftFoot);
        float legLength = Vector3.Distance(upper.position, lower.position) + Vector3.Distance(lower.position, foot.position);
        float avatarRatio = a.humanScale * Mathf.Abs(p.transform.lossyScale.y) / legLength;
        if (!float.IsFinite(avatarRatio) || avatarRatio < 0.25f || avatarRatio > 4f) throw new InvalidOperationException("Avatar scale does not match the imported skeleton; rebuild its skeleton description after changing import scale.");
        var sourceModel = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(a.avatar));
        var sourceAnimator = sourceModel == null ? null : sourceModel.GetComponent<Animator>();
        if (sourceAnimator != null)
        {
            float sourceLegLength = Vector3.Distance(sourceAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position, sourceAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position) + Vector3.Distance(sourceAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position, sourceAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).position);
            float skeletonRatio = (legLength / Mathf.Abs(p.transform.lossyScale.y)) / (sourceLegLength / Mathf.Abs(sourceModel.transform.lossyScale.y));
            if (Mathf.Abs(skeletonRatio - 1f) > 0.02f) throw new InvalidOperationException("Player skeleton scale differs from its source Avatar. Normalize through the FBX importer, not rig transforms.");
        }
        var anim = p.GetComponent<PlayerAnimationController>(); var fx = p.GetComponent<PlayerEffectPoints>();
        if (fx == null || Ref(anim, "axeTransform") == null || Ref(anim, "leftHandTarget") == null || Ref(anim, "itemTransform") == null) throw new InvalidOperationException("Missing sockets or effect component.");
        foreach (var component in new Component[] { anim, p.GetComponent<PlayerController>(), p.GetComponent<PlayerAxeSkinController>(), fx })
        {
            var so = new SerializedObject(component); var it = so.GetIterator();
            while (it.Next(true)) if (it.propertyType == SerializedPropertyType.ObjectReference)
            {
                var tr = it.objectReferenceValue as Transform;
                if (tr != null && tr != p.transform && !tr.IsChildOf(p.transform)) throw new InvalidOperationException("External transform reference: " + it.propertyPath);
            }
        }
        foreach (var field in new[] { "groundCheck", "pivotTransform", "_mainCamera", "firstPersonObjects" })
            if (new SerializedObject(p.GetComponent<PlayerController>()).FindProperty(field).objectReferenceValue == null) throw new InvalidOperationException("Missing player reference " + field);
        foreach (var field in new[] { "animator", "firstPersonTweenAnimator", "playerCamera", "itemMeshRenderer" })
            if (new SerializedObject(anim).FindProperty(field).objectReferenceValue == null) throw new InvalidOperationException("Missing animation reference " + field);
        if (!Ref(anim, "axeTransform").IsChildOf(a.GetBoneTransform(HumanBodyBones.RightHand))) throw new InvalidOperationException("Axe is not attached to the new right hand.");
        var body = BodyRoots(p);
        foreach (var r in body.SelectMany(g => g.GetComponentsInChildren<SkinnedMeshRenderer>(true)))
            if (r.sharedMesh == null || r.rootBone == null || r.bones.Length == 0 || r.bones.Any(b => b == null || !b.IsChildOf(p.transform))) throw new InvalidOperationException("Broken skinned mesh bones.");
        if (a.runtimeAnimatorController.animationClips.Length == 0) throw new InvalidOperationException("No gameplay animation clips.");
        foreach (string key in new[] { "Speed_X", "Speed_Z", "HitReady", "Hit" }) if (!a.parameters.Any(parameter => parameter.name == key)) throw new InvalidOperationException("Missing animation parameter " + key);
        foreach (var field in new[] { "head", "body", "foot", "rightHand", "leftHand", "axe" }) if (Ref(fx, field) == null) throw new InvalidOperationException("Missing effect anchor " + field);
    }
    private static void RenderPreviews(Report r)
    {
        var defaultClips = AssetDatabase.LoadAssetAtPath<GameObject>(r.prefabPath).GetComponent<Animator>().runtimeAnimatorController.animationClips;
        Render(r, defaultClips.FirstOrDefault(c => c.name.Contains("2Loop")), r.iconPath);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(r.prefabPath);
        var clips = prefab.GetComponent<Animator>().runtimeAnimatorController.animationClips.Distinct().ToArray();
        foreach (var group in new[] { new[] { "walk", "_wc", "run" }, new[] { "hit", "strike", "chop", "_3h" } })
        {
            var clip = clips.FirstOrDefault(c => group.Any(k => c.name.ToLowerInvariant().Contains(k)));
            if (clip == null) { r.warnings.Add("Could not automatically identify animation preview for " + group[0] + ". Inspect controller clips manually."); continue; }
            string path = r.folder + "/Preview_" + group[0] + ".png";
            Render(r, clip, path); r.previews.Add(path);
        }
        if (r.previews.Count < 2) throw new InvalidOperationException("Movement and hit previews are required. Resolve clip selection before registration.");
    }
    private static void Render(Report r, AnimationClip clip, string path)
    {
        var utility = new PreviewRenderUtility(); Texture2D image = null; bool sampling = false;
        try
        {
            var player = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(r.prefabPath)); utility.AddSingleGO(player);
            foreach (var camera in player.GetComponentsInChildren<Camera>(true)) camera.enabled = false;
            foreach (var renderer in player.GetComponentsInChildren<Renderer>(true)) renderer.enabled = false;
            var bodies = BodyRoots(player); foreach (var renderer in bodies.SelectMany(g => g.GetComponentsInChildren<Renderer>(true))) { renderer.enabled = true; renderer.shadowCastingMode = ShadowCastingMode.On; }
            var animator = player.GetComponent<Animator>(); animator.Rebind(); animator.Update(0);
            var axeSkin = new SerializedObject(player.GetComponent<PlayerAxeSkinController>()).FindProperty("axeSkinCatalog").objectReferenceValue as AxeSkinCatalogSO;
            var axeRoot = Ref(player.GetComponent<PlayerAxeSkinController>(), "axeVisualRoot");
            AxeSkinSO defaultAxe;
            if (axeSkin != null && axeRoot != null && axeSkin.TryGetSkin("axe_basic", out defaultAxe) && defaultAxe.AxePrefab != null)
            {
                foreach (Transform child in axeRoot.Cast<Transform>().ToArray()) Object.DestroyImmediate(child.gameObject);
                var axeVisual = Object.Instantiate(defaultAxe.AxePrefab, axeRoot, false);
                axeVisual.transform.localPosition = Vector3.zero; axeVisual.transform.localRotation = Quaternion.identity; axeVisual.transform.localScale = Vector3.one;
            }
            foreach (var renderer in Ref(player.GetComponent<PlayerAnimationController>(), "axeTransform").GetComponentsInChildren<Renderer>(true)) renderer.enabled = true;
            if (clip != null)
            {
                AnimationMode.StartAnimationMode(); sampling = true;
                AnimationMode.BeginSampling(); AnimationMode.SampleAnimationClip(player, clip, clip.length * 0.45f); AnimationMode.EndSampling();
            }
            Bounds bounds = BoundsOf(bodies.SelectMany(g => g.GetComponentsInChildren<Renderer>(true)));
            utility.camera.fieldOfView = 30; utility.camera.nearClipPlane = 0.01f; utility.camera.farClipPlane = 1000;
            Vector3 direction = new Vector3(0.35f, 0.1f, 1).normalized;
            utility.camera.transform.position = bounds.center + direction * (bounds.size.magnitude * 1.65f);
            utility.camera.transform.LookAt(bounds.center);
            utility.camera.clearFlags = CameraClearFlags.SolidColor; utility.camera.backgroundColor = new Color(0.12f, 0.14f, 0.18f, 1);
            utility.ambientColor = new Color(0.6f, 0.6f, 0.6f);
            utility.lights[0].intensity = 2.5f; utility.lights[0].transform.rotation = Quaternion.Euler(35, 160, 0); utility.lights[1].intensity = 1.5f;
            utility.BeginStaticPreview(new Rect(0, 0, 512, 512)); utility.Render(true); image = utility.EndStaticPreview();
            File.WriteAllBytes(path, image.EncodeToPNG());
        }
        finally { if (sampling) AnimationMode.StopAnimationMode(); if (image != null) Object.DestroyImmediate(image); utility.Cleanup(); }
    }
}
