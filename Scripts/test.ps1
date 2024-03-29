param([string]$config = "Debug")

$testDirectory = ".test-results"
$nunitExe = (ls .\packages\*\tools\nunit-console.exe | select -First 1).FullName

if(!(Test-Path $testDirectory)) { 
    mkdir $testDirectory > $null 

    attrib.exe $testDirectory +h
}

ls "$testDirectory\*.test.xml" -re | rm -Force

ls "*.Test.dll" -Recurse | ? DirectoryName -match "bin\\$config" | % {

    $out = "$testDirectory\$($_.Name).test.xml"
    & $nunitExe /nologo /nodots /xml=$out $_.FullName

}

$results = @()

(ls "$testDirectory\*.test.xml") | % {

    $x = [xml](gc $_)

    $results += $x."test-results" | % {

        [pscustomobject]@{

            Name = ($_.name -split "\\" | select -last 1);
            Total = $_.total;
            Errors = [int]($_.errors);
            Failures = [int]($_.failures);
            Ignored = $_.ignored
        }
    }
}

$errors = ($results | % { $_.Errors }   | Measure -Sum).Sum

$results += [pscustomobject]@{
    Name     = "Total";
    Total    = ($results | % { $_.Total }    | Measure -Sum).Sum;
    Errors   = $errors;
    Failures = ($results | % { $_.Failures } | Measure -Sum).Sum;
    Ignored  = ($results | % { $_.Ignored }  | Measure -Sum).Sum;
}

$results | ft -AutoSize

exit $errors
