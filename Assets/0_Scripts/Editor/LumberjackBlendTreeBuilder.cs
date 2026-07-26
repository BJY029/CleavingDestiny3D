#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class LumberjackBlendTreeBuilder
{
    private const string ControllerPath = "Assets/1_Animation/NewPlayerAnimation.controller";
    private const string BasePath = "Assets/99_ImportAssets/Lumbering fbx/2HAxeVsTree/Loco";
    private const string IdlePath = "Assets/99_ImportAssets/Lumbering fbx/AS_Base.fbx";

    [MenuItem("Tools/Build Lumberjack BlendTree Controller")]
    public static void BuildController()
    {
        // 1. AnimatorController 생성 또는 기존 로드
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            Debug.Log($"[LumberjackBuilder] Controller 새로 생성됨: {ControllerPath}");
        }

        // 2. 파라미터 추가 (Speed_X, Speed_Z, IsReady, Hit)
        AddParameterIfNotExists(controller, "Speed_X", AnimatorControllerParameterType.Float);
        AddParameterIfNotExists(controller, "Speed_Z", AnimatorControllerParameterType.Float);
        AddParameterIfNotExists(controller, "IsReady", AnimatorControllerParameterType.Bool);
        AddParameterIfNotExists(controller, "Hit", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // 기존 State 및 BlendTree 정리 또는 새로 만들기
        AnimatorState idleState = FindOrCreateState(rootStateMachine, "Idle", new Vector3(300, -100, 0));
        AnimatorState startState = FindOrCreateState(rootStateMachine, "Loco_Start", new Vector3(300, 0, 0));
        AnimatorState loopState = FindOrCreateState(rootStateMachine, "Loco_Loop", new Vector3(300, 100, 0));
        AnimatorState endState = FindOrCreateState(rootStateMachine, "Loco_End", new Vector3(300, 200, 0));
        
        AnimatorState hitReadyState = FindOrCreateState(rootStateMachine, "HitReady", new Vector3(600, 0, 0));
        AnimatorState hitState = FindOrCreateState(rootStateMachine, "Hit", new Vector3(600, 100, 0));

        rootStateMachine.defaultState = idleState;

        // Idle 및 Hit 모션 설정
        AnimationClip idleClip = LoadAnimationClip(IdlePath);
        if (idleClip != null) idleState.motion = idleClip;

        AnimationClip hitReadyClip = LoadAnimationClip("Assets/99_ImportAssets/Lumbering fbx/2HAxeVsTree/AS_ToolAxe_2Loop.fbx");
        if (hitReadyClip != null) hitReadyState.motion = hitReadyClip;

        AnimationClip hitClip = LoadAnimationClip("Assets/99_ImportAssets/Lumbering fbx/2HAxeVsTree/AS_ToolAxeVStree_Top_3M.fbx");
        if (hitClip != null) hitState.motion = hitClip;

        // 3. BlendTree 구성 (Start, Loop, End) - 2HAxeVsTree 파일 구조 반영
        SetupBlendTree(controller, startState, "StartStep_BlendTree", "Walk/First Steps/AS_AxeTool_WFS", "Run/First Steps/AS_AxeTool_RFS", idleClip);
        SetupBlendTree(controller, loopState, "Loop_BlendTree", "Walk/AS_AxeTool_WC", "Run/AS_AxeTool_RC", idleClip);
        SetupBlendTree(controller, endState, "EndStep_BlendTree", "Walk/Lasts Steps/AS_AxeTool_WLS", "Run/Last Steps/AS_AxeTool_RLS", idleClip);

        // 4. Transitions 연결
        ClearTransitions(idleState);
        ClearTransitions(startState);
        ClearTransitions(loopState);
        ClearTransitions(endState);
        ClearTransitions(hitReadyState);
        ClearTransitions(hitState);

        // Idle -> StartState (Speed_X != 0 or Speed_Z != 0)
        var idleToStart = idleState.AddTransition(startState);
        idleToStart.hasExitTime = false;
        idleToStart.duration = 0.1f;
        idleToStart.AddCondition(AnimatorConditionMode.Greater, 0.05f, "Speed_X");

        var idleToStart2 = idleState.AddTransition(startState);
        idleToStart2.hasExitTime = false;
        idleToStart2.duration = 0.1f;
        idleToStart2.AddCondition(AnimatorConditionMode.Less, -0.05f, "Speed_X");

        var idleToStart3 = idleState.AddTransition(startState);
        idleToStart3.hasExitTime = false;
        idleToStart3.duration = 0.1f;
        idleToStart3.AddCondition(AnimatorConditionMode.Greater, 0.05f, "Speed_Z");

        var idleToStart4 = idleState.AddTransition(startState);
        idleToStart4.hasExitTime = false;
        idleToStart4.duration = 0.1f;
        idleToStart4.AddCondition(AnimatorConditionMode.Less, -0.05f, "Speed_Z");

        // StartState -> LoopState (Exit Time 기반)
        var startToLoop = startState.AddTransition(loopState);
        startToLoop.hasExitTime = true;
        startToLoop.exitTime = 0.85f;
        startToLoop.duration = 0.15f;

        // LoopState -> EndState (이동 멈춤)
        var loopToEnd = loopState.AddTransition(endState);
        loopToEnd.hasExitTime = false;
        loopToEnd.duration = 0.15f;
        loopToEnd.AddCondition(AnimatorConditionMode.Less, 0.05f, "Speed_X");
        loopToEnd.AddCondition(AnimatorConditionMode.Greater, -0.05f, "Speed_X");
        loopToEnd.AddCondition(AnimatorConditionMode.Less, 0.05f, "Speed_Z");
        loopToEnd.AddCondition(AnimatorConditionMode.Greater, -0.05f, "Speed_Z");

        // EndState -> Idle (Exit Time)
        var endToIdle = endState.AddTransition(idleState);
        endToIdle.hasExitTime = true;
        endToIdle.exitTime = 0.85f;
        endToIdle.duration = 0.15f;

        // AnyState -> HitReady (IsReady == True)
        var anyToHitReady = rootStateMachine.AddAnyStateTransition(hitReadyState);
        anyToHitReady.hasExitTime = false;
        anyToHitReady.duration = 0.1f;
        anyToHitReady.AddCondition(AnimatorConditionMode.If, 0, "IsReady");

        // HitReady -> Hit (Hit Trigger)
        var readyToHit = hitReadyState.AddTransition(hitState);
        readyToHit.hasExitTime = false;
        readyToHit.duration = 0.05f;
        readyToHit.AddCondition(AnimatorConditionMode.If, 0, "Hit");

        // Hit -> HitReady (HasExitTime = True)
        var hitToReady = hitState.AddTransition(hitReadyState);
        hitToReady.hasExitTime = true;
        hitToReady.exitTime = 0.9f;
        hitToReady.duration = 0.1f;
        hitToReady.AddCondition(AnimatorConditionMode.If, 0, "IsReady");

        // HitReady -> Idle (IsReady == False)
        var readyToIdle = hitReadyState.AddTransition(idleState);
        readyToIdle.hasExitTime = false;
        readyToIdle.duration = 0.15f;
        readyToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsReady");

        // 저장
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ [LumberjackBuilder] 성공적으로 애니메이터 컨트롤러 BlendTree 구축이 완료되었습니다!");
    }

    private static void SetupBlendTree(AnimatorController controller, AnimatorState state, string treeName, string walkPrefix, string runPrefix, AnimationClip defaultIdle)
    {
        BlendTree tree;
        controller.CreateBlendTreeInController(treeName, out tree, 0);
        tree.blendType = BlendTreeType.FreeformDirectional2D;
        tree.blendParameter = "Speed_X";
        tree.blendParameterY = "Speed_Z";

        // 1. Idle 노드 (0, 0)
        tree.AddChild(defaultIdle, new Vector2(0f, 0f));

        // 2. Walk 노드 (반지름 0.5)
        AddMotionNodes(tree, walkPrefix, 0.5f);

        // 3. Run 노드 (반지름 1.0)
        AddMotionNodes(tree, runPrefix, 1.0f);

        state.motion = tree;
    }

    private static void AddMotionNodes(BlendTree tree, string prefix, float radius)
    {
        // 8방향 파일 접미사 및 좌표 매핑
        var angles = new (string suffix, Vector2 pos)[]
        {
            ("", new Vector2(0f, radius)),                                 // 0도 (전방)
            ("45", new Vector2(radius * 0.7071f, radius * 0.7071f)),        // 45도 (우상단)
            ("90", new Vector2(radius, 0f)),                                // 90도 (우측)
            ("135", new Vector2(radius * 0.7071f, -radius * 0.7071f)),      // 135도 (우하단)
            ("180", new Vector2(0f, -radius)),                              // 180도 (후방)
            ("225", new Vector2(-radius * 0.7071f, -radius * 0.7071f)),     // 225도 (좌하단)
            ("270", new Vector2(-radius, 0f)),                               // 270도 (좌측)
            ("315", new Vector2(-radius * 0.7071f, radius * 0.7071f))       // 315도 (좌상단)
        };

        foreach (var (suffix, pos) in angles)
        {
            string path = $"{BasePath}/{prefix}{suffix}.fbx";
            AnimationClip clip = LoadAnimationClip(path);
            if (clip != null)
            {
                tree.AddChild(clip, pos);
            }
            else
            {
                Debug.LogWarning($"[LumberjackBuilder] 애니메이션 클립을 찾을 수 없음: {path}");
            }
        }
    }

    private static AnimationClip LoadAnimationClip(string fbxPath)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        if (assets == null || assets.Length == 0) return null;

        return assets.OfType<AnimationClip>()
                     .FirstOrDefault(c => !c.name.StartsWith("__preview__"));
    }

    private static void AddParameterIfNotExists(AnimatorController controller, string paramName, AnimatorControllerParameterType type)
    {
        if (!controller.parameters.Any(p => p.name == paramName))
        {
            controller.AddParameter(paramName, type);
        }
    }

    private static AnimatorState FindOrCreateState(AnimatorStateMachine sm, string stateName, Vector3 position)
    {
        ChildAnimatorState childState = sm.states.FirstOrDefault(s => s.state.name == stateName);
        if (childState.state != null)
        {
            return childState.state;
        }
        AnimatorState newState = sm.AddState(stateName, position);
        return newState;
    }

    private static void ClearTransitions(AnimatorState state)
    {
        var transitions = state.transitions.ToArray();
        foreach (var t in transitions)
        {
            state.RemoveTransition(t);
        }
    }
}
#endif
