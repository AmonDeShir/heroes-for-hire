$ErrorActionPreference = 'Stop'

function Strip-CSharpComments([string] $text) {
  $sb = New-Object System.Text.StringBuilder

  $i = 0
  $len = $text.Length

  $inString = $false
  $inVerbatimString = $false
  $inChar = $false
  $inLineComment = $false
  $inBlockComment = $false

  while ($i -lt $len) {
    $c = $text[$i]

    if ($inLineComment) {
      if ($c -eq "`n") {
        $inLineComment = $false
        [void]$sb.Append($c)
      }
      $i++
      continue
    }

    if ($inBlockComment) {
      if ($c -eq '*' -and ($i + 1) -lt $len -and $text[$i + 1] -eq '/') {
        $inBlockComment = $false
        $i += 2
        continue
      }
      $i++
      continue
    }

    if ($inString) {
      [void]$sb.Append($c)
      if ($c -eq '"') {
        $inString = $false
      } elseif ($c -eq '\\' -and ($i + 1) -lt $len) {
        # escape sequence inside regular string
        $i++
        [void]$sb.Append($text[$i])
      }
      $i++
      continue
    }

    if ($inVerbatimString) {
      [void]$sb.Append($c)
      if ($c -eq '"') {
        if (($i + 1) -lt $len -and $text[$i + 1] -eq '"') {
          # doubled quote inside verbatim string
          $i++
          [void]$sb.Append($text[$i])
        } else {
          $inVerbatimString = $false
        }
      }
      $i++
      continue
    }

    if ($inChar) {
      [void]$sb.Append($c)
      if ($c -eq "'") {
        $inChar = $false
      } elseif ($c -eq '\\' -and ($i + 1) -lt $len) {
        $i++
        [void]$sb.Append($text[$i])
      }
      $i++
      continue
    }

    # Not currently in string/char/comment. Detect string starts.
    if ($c -eq '@' -and ($i + 1) -lt $len -and $text[$i + 1] -eq '"') {
      $inVerbatimString = $true
      [void]$sb.Append('@')
      $i++
      [void]$sb.Append('"')
      $i++
      continue
    }
    if ($c -eq '"') {
      $inString = $true
      [void]$sb.Append($c)
      $i++
      continue
    }
    if ($c -eq "'") {
      $inChar = $true
      [void]$sb.Append($c)
      $i++
      continue
    }

    # Detect comments.
    if ($c -eq '/' -and ($i + 1) -lt $len) {
      $n = $text[$i + 1]
      if ($n -eq '/') {
        # Keep XML doc comments (///) intact.
        if (($i + 2) -lt $len -and $text[$i + 2] -eq '/') {
          [void]$sb.Append("///")
          $i += 3
          continue
        }
        $inLineComment = $true
        $i += 2
        continue
      }
      if ($n -eq '*') {
        $inBlockComment = $true
        $i += 2
        continue
      }
    }

    [void]$sb.Append($c)
    $i++
  }

  return $sb.ToString()
}

$root = Join-Path $PSScriptRoot '..\Assets\_heroes'
$excludeMarker = "\\Art\\Packs\\"

$files = Get-ChildItem -Path $root -Recurse -File -Filter '*.cs' |
  Where-Object {
    $full = $_.FullName.Replace('/', '\\')
    return ($full.IndexOf($excludeMarker, [System.StringComparison]::OrdinalIgnoreCase) -lt 0)
  }

$changed = 0
foreach ($f in $files) {
  $text = [System.IO.File]::ReadAllText($f.FullName)
  $newText = Strip-CSharpComments $text
  # Repair: previous buggy runs could leave lines with a single '/'
  # when encountering '///'. Those should never be valid C# on their own.
  $lines = $newText -split "`r?`n"
  $lines = $lines | Where-Object { $_ -notmatch '^\s*/\s*$' }
  $newText = ($lines -join "`r`n") + "`r`n"
  if ($newText -ne $text) {
    [System.IO.File]::WriteAllText($f.FullName, $newText)
    $changed++
  }
}

Write-Host "Stripped comments in $changed file(s)."
