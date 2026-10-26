$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path -Parent $PSScriptRoot
$source = Join-Path $taskRoot 'Tools/Skills/unity-character-onboarding'
$target = Join-Path $env:USERPROFILE '.codex/skills/unity-character-onboarding'
$validator = Join-Path $env:USERPROFILE '.codex/skills/.system/skill-creator/scripts/quick_validate.py'
$dependencyPath = Join-Path $taskRoot 'Temp/skill-validator-deps'
$oldPythonPath = $env:PYTHONPATH
try {
    $env:PYTHONPATH = $dependencyPath
    & python $validator $source
    if ($LASTEXITCODE -ne 0) { throw 'Skill validation failed.' }
    if (Test-Path -LiteralPath $target) {
        foreach ($relative in @('SKILL.md', 'agents/openai.yaml', 'references/workflow.md', 'scripts/CharacterOnboarding.cs')) {
            $installed = Join-Path $target $relative
            if (!(Test-Path -LiteralPath $installed) -or (Get-FileHash -LiteralPath $installed).Hash -ne (Get-FileHash -LiteralPath (Join-Path $source $relative)).Hash) { throw "Existing skill differs; refusing to overwrite: $installed" }
        }
        Write-Output "Already installed: $target"
    } else {
        New-Item -ItemType Directory -Path (Split-Path -Parent $target) -Force | Out-Null
        Copy-Item -LiteralPath $source -Destination $target -Recurse
        Write-Output "Installed: $target"
    }
    & python $validator $target
    if ($LASTEXITCODE -ne 0) { throw 'Installed skill validation failed.' }
} finally { $env:PYTHONPATH = $oldPythonPath }
