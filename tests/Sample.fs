module QualIQon.Tests

open Expecto
open QualIQon

[<Tests>]
let tests =
    testList "Misscleavages" [
        testCase "relativeDistribution sums to 1" <| fun _ ->
            let data =
                [|
                    { Misscleavages.Parameters.MC = 0 }
                    { MC = 1 }
                    { MC = 1 }
                |]

            let dist = Misscleavages.relativeDistribution data
            let sum = dist |> Array.sumBy snd
            Expect.floatClose Accuracy.high sum 1.0 "should sum to 1"
    ]
