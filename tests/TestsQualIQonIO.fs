module TestsIO

open Expecto
open QualIQon.IO 
open System
open System.IO

[<Tests>] 
let tests = 
    testList "QualIQon.IO"  [

        testCase "getFiles ProteomIQon returns *.quant for CorrHeavy" <| fun _ ->
            let pattern = QualIQon.IO.CorrHeavy.getFiles "ProteomIQon"
            Expect.equal pattern "*.quant" "Pattern should match *.quant"

        testCase "getFiles ProteomIQon returns *.quant for CorrLight" <| fun _ ->
            let pattern = QualIQon.IO.CorrLight.getFiles "ProteomIQon"
            Expect.equal pattern "*.quant" "Pattern should match *.quant"

        testCase "getFiles ProteomIQon returns *.quant for CorrRatio" <| fun _ ->
            let pattern = QualIQon.IO.CorrRatio.getFiles "ProteomIQon"
            Expect.equal pattern "*.quant" "Pattern should match *.quant"

        testCase "getFiles ProteomIQon returns *.quant for Correlation Light Heavy" <| fun _ ->
            let pattern = QualIQon.IO.CorrelationLightHeavy.getFiles "ProteomIQon"
            Expect.equal pattern "*.quant" "Pattern should match *.quant"

        testCase "getFiles ProteomIQon returns *.qpsm for Misscleavages" <| fun _ ->
            let pattern = QualIQon.IO.Misscleavages.getFiles "ProteomIQon"
            Expect.equal pattern "*.qpsm" "Pattern should match *.qpsm"

        testCase "getFiles ProteomIQon returns *.prot for Protein Identification" <| fun _ ->
            let pattern = QualIQon.IO.FilesProteinIdentification.matchingDirecoryPath "ProteomIQon"
            Expect.equal pattern "*.prot" "Pattern should match *.prot"

        testCase "TIC returns empty array for empty directory" <| fun _ ->
            // temp directory ohne mzML files
            let dir =
                System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    System.Guid.NewGuid().ToString("N")
                )

            System.IO.Directory.CreateDirectory(dir) |> ignore

            let result = QualIQon.IO.TICFiles.TIC dir

            Expect.equal result.Length 0 "No mzML files -> no TIC data"

        testCase "XIC returns empty array for empty directory" <| fun _ ->
            // temp directory ohne mzML files
            let dir =
                System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    System.Guid.NewGuid().ToString("N")
                )

            System.IO.Directory.CreateDirectory(dir) |> ignore

            let result = QualIQon.IO.XICFiles.XIC dir

            Expect.equal result.Length 0 "No mzML files -> no XIC data"

        testCase "MS1Map returns empty array for empty directory" <| fun _ ->
            // temp directory ohne mzML files
            let dir =
                System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    System.Guid.NewGuid().ToString("N")
                )

            System.IO.Directory.CreateDirectory(dir) |> ignore

            let result = QualIQon.IO.MS1MapFiles.MS1Map dir

            Expect.equal result.Length 0 "No mzML files -> no MS1Map data"
]
