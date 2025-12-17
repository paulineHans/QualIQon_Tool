module QualIQon.IO.Tests.CorrHeavyTests

open Expecto
open System.IO
open Deedle
open QualIQon.IO.CorrHeavy

[<Tests>]
let corrHeavyTests =
    testList "CorrHeavy Module Tests" [
        
        test "Dummy test to check setup" {
        let result = 1 + 1
        Expect.equal result 2 "1 + 1 should be 2"
        }

        test "drainData parses CSV correctly" {
            // Beispiel-CSV erstellen
            let tmpFile = Path.GetTempFileName()
            File.WriteAllText(tmpFile, """
    StringSequence	Charge	GlobalMod	Quant_Heavy
    AAA	2	true	10
    BBB	3	false	20
    """.Trim())

            let fileName, data = drainData tmpFile

            // Test: Datei-Name wird korrekt extrahiert
            Expect.isTrue (fileName.Contains(Path.GetFileNameWithoutExtension(tmpFile))) "Filename should match"

            // Test: Daten korrekt eingelesen
            Expect.equal (data.Length) 2 "Should have 2 data rows"
            Expect.sequenceEqual (data |> Array.map snd) [|10.0; 20.0|] "Quant_Heavy values should match"

            // Temp-Datei löschen
            File.Delete(tmpFile)
        ]

        test "pipelineUsed returns drainData for ProteomIQon" {
            let func = pipelineUsed "ProteomIQon"
            let tmpFile = Path.GetTempFileName()
            File.WriteAllText(tmpFile, """
    StringSequence	Charge	GlobalMod	Quant_Heavy
    AAA	2	true	10
    """.Trim())

            let _, data = func tmpFile
            Expect.equal (data.[0] |> snd) 10.0 "Should read Quant_Heavy correctly"

            File.Delete(tmpFile)
        }

    ]

[<EntryPoint>]
let main argv =
    runTestsWithArgs defaultConfig argv corrHeavyTests
