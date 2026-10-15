param([string]$out)

if (-not $out) { Write-Error 'Output path argument required'; exit 1 }

if (-not (Test-Path $out)) {
	$sampleRate = 44100
	$bits = 16
	$channels = 1
	$duration = 0.9
	$samples = [int]($sampleRate * $duration)
	$freq = 880.0
	$amplitude = 0.25 * [int]::MaxValue

	$dir = Split-Path $out -Parent
	if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }

	$fs = [System.IO.File]::Open($out, 'Create')
	$bw = New-Object System.IO.BinaryWriter($fs)

	$byteRate = $sampleRate * $channels * $bits / 8
	$subChunk2 = $samples * $channels * $bits / 8

	$bw.Write([System.Text.Encoding]::ASCII.GetBytes("RIFF"))
	$bw.Write([int](36 + $subChunk2))
	$bw.Write([System.Text.Encoding]::ASCII.GetBytes("WAVE"))

	$bw.Write([System.Text.Encoding]::ASCII.GetBytes("fmt "))
	$bw.Write(16)
	$bw.Write([short]1)
	$bw.Write([short]$channels)
	$bw.Write([int]$sampleRate)
	$bw.Write([int]$byteRate)
	$bw.Write([short]($channels * $bits / 8))
	$bw.Write([short]$bits)

	$bw.Write([System.Text.Encoding]::ASCII.GetBytes("data"))
	$bw.Write([int]$subChunk2)

	for ($i = 0; $i -lt $samples; $i++) {
		$t = $i / $sampleRate
		$s = [int]([math]::Round($amplitude * [math]::Sin(2 * [math]::PI * $freq * $t)))
		$bw.Write([short]$s)
	}

	$bw.Close()
}
