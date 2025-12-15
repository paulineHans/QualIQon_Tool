namespace QualIQon.IO

open System
open System.IO
open Deedle 


module CorrHeavy = 

    let drainData (allData : string array) = 
        let fileNames = allData |> Array.map (fun x -> x |> System.IO.Path.GetFileNameWithoutExtension)
        let files = 
            Deedle.Frame.ReadCsv(allData,true,true,separators="\t")
        let data = 
            files 
            |> Frame.indexRowsUsing (fun x -> 
                x.GetAs<string>("StringSequence"),
                x.GetAs<int>("Charge"),
                x.GetAs<bool>("GlobalMod")
            )
            |> Frame.getCol "Quant_Heavy"
            |> Series.observations
            |> Seq.toArray
            |> Array.map (fun (x,y) -> x, y |> float)
        fileNames, data

    let pipelineUsed (arguments: string) =  
        match arguments with 
        | "ProteomIQon" -> drainData 
        | _ -> failwith "unknown Pattern"

    let rec searchFiles (directoryName: string) (fileName: string) : string[] =
        // Get files in the current directory that match the filename.
        let currentFiles = Directory.GetFiles(directoryName,fileName )
        
        // Get all subdirectories.
        let subDirectories = Directory.GetDirectories(directoryName)
        
        // Recursively search each subdirectory.
        let subDirFiles =
            subDirectories
            |> Array.collect (fun subDir -> searchFiles subDir fileName)
        
        Array.append currentFiles subDirFiles
    
    let getFiles (sndArg : string) = 
        match sndArg with
        | "ProteomIQon" -> "*.quant"
        | _ -> "unknown Format"

    let finalHeatmapQuantHeavy (directoryName: string) pipeline =
            let searchpattern = getFiles pipeline
            let allData = searchFiles  directoryName searchpattern
            (pipelineUsed pipeline) allData
    

 
     


        