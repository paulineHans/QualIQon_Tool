namespace QualIQon.IO

open ProteomIQon.Dto
open FSharpAux.IO.SchemaReader.Attribute
open MzIO.IO.MzML
open MzIO.Processing
open MzIO.Model
open MzIO
open ProteomIQon
open BioFSharp
open BioFSharp.IO
open BioFSharp.PeptideClassification
open System
open System.IO
open Deedle 



type QuantificationResult = {
    [<FieldAttribute(0)>]
    StringSequence                              : string
    [<FieldAttribute(1)>]
    GlobalMod                                   : int
    [<FieldAttribute(2)>]
    Charge                                      : int
    [<FieldAttribute(3)>]
    PepSequenceID                               : int
    [<FieldAttribute(4)>]
    ModSequenceID                               : int
    [<FieldAttribute(5)>]
    PrecursorMZ                                 : float
    [<FieldAttribute(6)>]
    MeasuredMass                                : float 
    [<FieldAttribute(7)>]
    TheoMass                                    : float
    [<FieldAttribute(8)>]
    AbsDeltaMass                                : float
    [<FieldAttribute(9)>]
    MeanPercolatorScore                         : float
    [<FieldAttribute(10)>]
    QValue                                      : float
    [<FieldAttribute(11)>]
    PEPValue                                    : float
    [<FieldAttribute(12)>]
    ProteinNames                                : string
    [<FieldAttribute(13)>]
    QuantMz_Light                               : float
    [<FieldAttribute(14)>]
    Quant_Light                                 : float
    [<FieldAttribute(15)>]
    MeasuredApex_Light                          : float 
    [<FieldAttribute(16)>]
    Seo_Light                                   : float
    [<FieldAttribute(17)>][<TraceConverter>]
    Params_Light                                : float []
    [<FieldAttribute(18)>]
    Difference_SearchRT_FittedRT_Light          : float
    [<FieldAttribute(19)>]
    KLDiv_Observed_Theoretical_Light            : float
    [<FieldAttribute(20)>]
    KLDiv_CorrectedObserved_Theoretical_Light   : float
    [<FieldAttribute(21)>]
    QuantMz_Heavy                               : float
    [<FieldAttribute(22)>]
    Quant_Heavy                                 : float
    [<FieldAttribute(23)>]
    MeasuredApex_Heavy                          : float
    [<FieldAttribute(24)>]
    Seo_Heavy                                   : float
    [<FieldAttribute(25)>][<TraceConverter>]
    Params_Heavy                                : float []        
    [<FieldAttribute(26)>]
    Difference_SearchRT_FittedRT_Heavy          : float
    [<FieldAttribute(27)>]
    KLDiv_Observed_Theoretical_Heavy            : float
    [<FieldAttribute(28)>]
    KLDiv_CorrectedObserved_Theoretical_Heavy   : float
    [<FieldAttribute(29)>]
    Correlation_Light_Heavy                     : float
    [<FieldAttribute(30)>][<QuantSourceConverter>]
    QuantificationSource                        : QuantificationSource
    [<FieldAttribute(31)>][<TraceConverter>]
    IsotopicPatternMz_Light                     : float []
    [<FieldAttribute(32)>][<TraceConverter>]
    IsotopicPatternIntensity_Observed_Light     : float []
    [<FieldAttribute(33)>][<TraceConverter>]
    IsotopicPatternIntensity_Corrected_Light    : float []
    [<FieldAttribute(34)>][<TraceConverter>]
    RtTrace_Light                               : float []
    [<FieldAttribute(35)>][<TraceConverter>]
    IntensityTrace_Observed_Light               : float []
    [<FieldAttribute(36)>][<TraceConverter>]
    IntensityTrace_Corrected_Light              : float []
    [<FieldAttribute(37)>][<TraceConverter>]
    IsotopicPatternMz_Heavy                     : float []
    [<FieldAttribute(38)>][<TraceConverter>]
    IsotopicPatternIntensity_Observed_Heavy     : float []
    [<FieldAttribute(39)>][<TraceConverter>]
    IsotopicPatternIntensity_Corrected_Heavy    : float []
    [<FieldAttribute(40)>][<TraceConverter>]
    RtTrace_Heavy                               : float []
    [<FieldAttribute(41)>][<TraceConverter>]
    IntensityTrace_Observed_Heavy               : float []
    [<FieldAttribute(42)>][<TraceConverter>]
    IntensityTrace_Corrected_Heavy              : float []
    [<FieldAttribute(43)>]
    AlignmentScore                              : float
    [<FieldAttribute(44)>]
    AlignmentQValue                             : float
}

type ParametersCorrLightHeavy = 
    {
        R: float
    }
type ParametersMisscleavages = 
    {
        MC : int
    }
type pieces = {
    RetentionTime:  float;
    mz:             float; 
    intensity:      float  
    }
type parameterEvidence = 
    {
        Group:  string
        Class: PeptideEvidenceClass 
    }

type parameters_PSMS = 
    {
        SND: float 
        AND: float 
        SNR: int 
        SQ: float 
        L: int
        PSMID: string

    }
type parameters_PSM = 
    {
        QV: float
        PEP: float 
    }

module CorrHeavy = 

    let drainData (allData : string ) = 
        let fileNames = System.IO.Path.GetFileNameWithoutExtension allData
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
            Array.map (pipelineUsed pipeline) allData

module CorrLight = 

    let drainData (allData : string ) = 
        let fileNames = System.IO.Path.GetFileNameWithoutExtension allData
        let files = 
            Deedle.Frame.ReadCsv(allData,true,true,separators="\t")
        let data = 
            files 
            |> Frame.indexRowsUsing (fun x -> 
                x.GetAs<string>("StringSequence"),
                x.GetAs<int>("Charge"),
                x.GetAs<bool>("GlobalMod")
            )
            |> Frame.getCol "Quant_Light"
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
            Array.map (pipelineUsed pipeline) allData

module CorrRatio = 

    let drainData (allData : string ) = 
        let allFiles = 
            let fileNamesL = System.IO.Path.GetFileNameWithoutExtension allData
            let files = 
                Deedle.Frame.ReadCsv(allData,true,true,separators="\t")
            let data = 
                files
                |> Frame.indexRowsUsing (fun x -> 
                    x.GetAs<string>("StringSequence"),
                    x.GetAs<int>("Charge"),
                    x.GetAs<bool>("GlobalMod")
                )
            let extractLight: Series<string*int*bool,float> = 
                data
                |> Frame.getCol "Quant_Light"
            let extractHeavy: Series<string*int*bool,float> = 
                data 
                |> Frame.getCol "Quant_Heavy"
            let divide = 
                extractLight/extractHeavy
                |> Series.observations
                |> Seq.toArray
                |> Array.map (fun (x,y)-> x, y |> float)
            fileNamesL, divide 
        allFiles

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
            Array.map (pipelineUsed pipeline) allData



module CorrelationLightHeavy = 
    let values (allData: string) = 
            let fileNames = System.IO.Path.GetFileNameWithoutExtension allData
            let files = 
                FSharpAux.IO.SchemaReader.Csv.CsvReader<QuantificationResult>().ReadFile(allData, '\t', false, 1)
                |> Seq.toArray
                |> Array.map (fun x -> 
                    { 
                        R = x.Correlation_Light_Heavy
                    }
                )
            (fileNames,files |> Seq.toArray)

    let pipelineUsed (arguments: string) =  
        match arguments with 
        | "ProteomIQon" -> values 
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
            Array.map (pipelineUsed pipeline) allData


module Misscleavages = 
    let proteomIQonToParams (path: string) =
        let fileNames = System.IO.Path.GetFileNameWithoutExtension path
        let files = 
            FSharpAux.IO.SchemaReader.Csv.CsvReader<PSMStatisticsResult>().ReadFile(path, '\t', false, 1)
            |> Seq.toArray
            |> Array.map (fun x -> 
                {
                    MC = x.MissCleavages
                }
            )  
        fileNames,files |> Seq.toArray 

        

    //get the data out of a FragPipe File
    let FragPipeToParams (path :string)= 
        let fileNames = System.IO.Path.GetFileNameWithoutExtension path
        let files = Deedle.Frame.ReadCsv (path, hasHeaders =  true,  separators = "\t")
        let columns = 
            files 
            |> Frame.getCol ("Number of Missed Cleavages")
            |> Series.values
            |> Seq.map (fun o -> 
                {
                MC = o
                } 

            )
            |> Seq.toArray 
        fileNames,columns

    let maxQuantToParams (path: string) = 
        let fileNames = System.IO.Path.GetFileNameWithoutExtension path 
        let files = Deedle.Frame.ReadCsv (path, hasHeaders =  true,  separators = "\t")
        let columns = 
            files 
            |> Frame.getCol ("Missed cleavages")
            |> Series.values
            |> Seq.map (fun x -> 
                {
                    MC = x
                }
            )
            |> Seq.toArray
        fileNames, columns

    let pipelineUsed (arguments: string) =  
        match arguments with 
        | "ProteomIQon" -> proteomIQonToParams
        | "FragPipe" -> FragPipeToParams
        | "MaxQuant" -> maxQuantToParams
        | _ -> failwith "unknown pattern"

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
        | "FragPipe" -> "psm.tsv"
        | "MaxQuant" -> "evidence.txt"
        | _ -> failwith "no file found"

    let finalHeatmapQuantHeavy (directoryName: string) pipeline =
            let searchpattern = getFiles pipeline
            let allData = searchFiles  directoryName searchpattern
            Array.map (pipelineUsed pipeline) allData







module ProteinIdentification = 
    let readFasta (path: string) =
        let readIn = 
            path
            |> FastA.fromFile BioArray.ofAminoAcidString
            |> Seq.filter (fun x -> x.Header.Contains "rev_" |> not) //filters decoys out
            |> Seq.toArray 
        let checktLength = 
            readIn
            |> Array.length 
        readIn
    
    let proteomIQonToParams (path: string array ) =   
        //function to get a) amount of identified proteins b) class
        let files =  
            let initialization =
                path
                |> Array.map (fun x ->
                    FSharpAux.IO.SchemaReader.Csv.CsvReader<ProteinInferenceResult>().ReadFile(x, '\t', false, 1)
                    |> Seq.toArray
                    |> Array.map (fun x -> 
                        {
                            Group = x.ProteinGroup
                            Class = x.Class
                        }
                    )
                    |> Array.map (fun x -> x.Group, x.Class))
            let expand = 
                initialization
                |> Array.collect (fun x -> 
                    x
                    |> Array.collect (fun (elements, value)  -> 
                        elements.Split(';') |> Array.map (fun elem -> elem, value)))
                |> Array.distinctBy fst
            let getClasses = 
                expand 
                |> Array.map (fun (x,y) -> y.ToString())
                |> Array.countBy id 
            getClasses
        files

    let maxQuantToParams (path: string array ) = 
        let files = 
            path
            |> Array.map (fun x -> 
                Deedle.Frame.ReadCsv (x, hasHeaders =  true,  separators = "\t"))
        let columns = 
            files
            |> Array.map (fun x -> 
                let getColumnfileNames = 
                    //names of identifed proteins in total 2711 Proteins
                    let prots = 
                        x
                        |> Frame.getCol "Protein names"
                        |> Series.values
                        |> Seq.map (unbox<string>)
                        |> Seq.toArray 
                // number of peptides that can be part of one or more proteins in total we have 21 
                // peptides that were identified for belonging to one or more proteins
                    let iProts =
                        x
                        |> Frame.getCol "Protein IDs"
                        |> Series.values
                        |> Seq.map (unbox<string>)
                        |> Seq.toArray
                        |> Array.filter (fun x -> x =  "" |> not)
                //expand
                    let transformArray  =
                        iProts 
                        |> Array.collect (fun elements -> 
                        elements.Split(';') |> Array.map (fun elem -> elem)
                        )
                        |> Array.append prots 
                    //calc length of combined array
                    let calc = 
                        transformArray 
                        |> Array.distinct
                        |> Array.length
                    "identfied Proteins", calc
                getColumnfileNames)
        columns 
    //maxQuantToParams "/home/paulinehans/Dokumente/QualIQon_v1.0/runs/yeast/MaxQuantFiles/proteinGroups.txt"

    let FragPipeToParams (path :string array)= 
        let files = 
            path 
            |> Array.map (fun x -> 
                        Deedle.Frame.ReadCsv (x, hasHeaders =  true,  separators = "\t")
            )
        let columns = 
            files 
            |> Array.map (fun x -> 
                let getColumnfileNames = 
                    //names of identifed proteins in total 2711 Proteins
                    let prots = 
                        x
                        |> Frame.getCol "Protein"
                        |> Series.values
                        |> Seq.map (unbox<string>)
                        |> Seq.toArray 
                    // number of peptides that can be part of one or more proteins in total we have 21 
                    // peptides that were identified for belonging to one or more proteins
                    let iProts =
                        x
                        |> Frame.getCol "Indistinguishable Proteins"
                        |> Series.values
                        |> Seq.map (unbox<string>)
                        |> Seq.toArray
                        |> Array.filter (fun x -> x =  "" |> not)
                    //expand
                    let transformArray  =
                        iProts
                        |> Array.collect (fun (elements) -> 
                        elements.Split(';') |> Array.map (fun elem -> elem)
                        )
                    let combine = transformArray |> Array.append prots 
                    let calc = 
                        combine 
                        |> Array.distinct
                        |> Array.length
                    "identifed Proteins", calc 
                getColumnfileNames)
        columns 
    
    let matchingForFiles (arguments : string) = 
        match arguments with 
        | "FragPipe" -> FragPipeToParams
        | "MaxQuant" -> maxQuantToParams
        | "ProteomIQon" -> proteomIQonToParams
        | _ -> failwith "failure"
    
    let rec searchFiles (directoryName: string) (fileName: string) : string[] =
        // Get files in the current directory that match the filename.
        let currentFiles = Directory.GetFiles(directoryName,fileName )
        
        // Get all subdirectories.
        let subDirectories = Directory.GetDirectories(directoryName)
        
        // Recursively search each subdirectory.
        let subDirFiles =
            subDirectories
            |> Array.collect (fun subDir -> searchFiles subDir fileName)
        
        // Combine files from the current directory and all subdirectories.
        Array.append currentFiles subDirFiles


    let matchingDirecoryPath (sndArg : string) = 
        match sndArg with
        | "ProteomIQon" -> "*.prot"
        | "FragPipe" -> "protein.tsv"
        | "MaxQuant" -> "proteinGroups.txt"
        | _ -> "fail"


    //main function
    let finalChartHisto (directoryName :string) pipeline fasta  =
        //callt function with pipeline parameter, which is the equivalent to sndArg
        let searchpattern  = matchingDirecoryPath pipeline 
        //edit fasta file  
        let FastaArrayLength = readFasta fasta 
        let lengthParameter = FastaArrayLength |> Array.length 
        //called searchFile function with directroypath and searchpattern 
        let allData  = searchFiles directoryName searchpattern
        let paramsArray =
            allData 
            |> fun x ->  matchingForFiles pipeline x
        paramsArray



module ScoreRefinement =
    let proteomIQonToParams (path:string) =
            let fileNames =  System.IO.Path.GetFileNameWithoutExtension path
            let files =  
                FSharpAux.IO.SchemaReader.Csv.CsvReader<PeptideSpectrumMatchingResult>().ReadFile(path, '\t', false, 1)
                |> Seq.toArray
                |> Array.map (fun x -> 
                    {
                        SND = x.SequestNormDeltaNext
                        AND = x.AndroNormDeltaNext
                        SNR = x.ScanNr
                        SQ = x.SequestScore
                        L = x.Label
                        PSMID = x.PSMId
                    }
                )
            (fileNames,files |> Seq.toArray)

        //read-in files 
    let finalChart (directoryName: string) =
        let allData  = System.IO.Directory.GetFiles (directoryName, "*.psm")
        let paramsArray = 
            allData 
            |> Array.map proteomIQonToParams
            |> Array.sort
        paramsArray

    let proteomIQonToParams_2 (path : string) = 
            let fileNames = System.IO.Path.GetFileNameWithoutExtension path
            let files = 
                FSharpAux.IO.SchemaReader.Csv.CsvReader<PSMStatisticsResult>().ReadFile(path, '\t', false, 1)
                |> Seq.toArray 
                |> Array.map (fun x -> 
                    {
                        QV = x.QValue
                        PEP = x.PEPValue
                    }
                )
            (fileNames,files |> Seq.toArray)

        //read-in files 
    let readingFiles (directoryName: string) =
        let allData2  = System.IO.Directory.GetFiles (directoryName, "*.qpsm")
        let paramsArray = 
            allData2
            |> Array.map proteomIQonToParams_2
            |> Array.sort 
        paramsArray




//MS1Map, TIC, XIC
module MassspecFiles = 
     let files (directorypath: string) =
        let allData  = System.IO.Directory.GetFiles (directorypath, "*.mzML")
        let exe = 
            allData
            |> Array.map (fun x -> 
            let inReaderMS = new MzMLReader(x)
            let inReaderPeaks = new MzMLReader(x)
            let inRunID  = Core.MzIO.Reader.getDefaultRunID inReaderMS 
            let allSpectra = inReaderMS.ReadMassSpectra inRunID
            let getMassSpecData  = 
                allSpectra
                |> Seq.choose (fun ms ->
                    match MzIO.Processing.MassSpectrum.getMsLevel ms with
                    | 1 -> 
                        Some(
                            MzIO.Processing.MassSpectrum.getScanTime ms,
                            PeakArray.mzIntensityArrayOf
                                ((inReaderPeaks).getSpecificPeak1DArraySequential ms.ID)
                        )
                    | _ -> None
                )
            getMassSpecData)
        exe

