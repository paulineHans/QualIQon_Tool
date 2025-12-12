namespace QualIQon 

open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open ProteomIQon.Dto
open Plotly.NET
open System
open BioFSharp
open BioFSharp.IO
open BioFSharp.PeptideClassification

module ProteinIdentification = 
    let customCulture: Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() :?> Globalization.CultureInfo
    customCulture.NumberFormat.NumberDecimalSeparator <- "."
    System.Threading.Thread.CurrentThread.CurrentCulture <- customCulture

    type parameter = 
        {
            Group:  string
            Class: PeptideEvidenceClass 
        }

    // read-in fasta file and get the length
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
    //FragPipeToParams "runs/yeast/FragPipeFiles/protein.tsv"


    //here the PeptideClasses are collectet and then the sum is calc

    let dataPeptideEvidenceClass (data :  (string*int) array) (allProteins: int)  = 
        let amountProteinsInFASTA = allProteins.ToString() 
        let nameForAbove = "unidentified Proteins (14140)"
        let sub = 
            let subs = Array.fold (fun x y -> x - snd y) allProteins  data
            let concat = [|nameForAbove, subs|]
            concat
        let com = 
            sub 
            |> Array.append data 
        com 
    
    dataPeptideEvidenceClass

    //chart building for all three aplications 
    let createChart (input : (string*int) array)  =     
        // let legendvalues1  = 
        //     input 
        //     |> Array.map (fun (x,y)-> y.ToString())
        //     |> Array.countBy id 
        // let test = 
        //     input 
        //     |> Array.map (fun x  -> 
        //         let getabovestring= 
        //             legendvalues1
        //             |> Array.map (fun y -> fst y , fst x) 
        //         getabovestring)
        //     |> Array.concat 
        // printfn "%A" test 
        let legendvalues = 
            input 
            |> Array.map fst
        let values = 
            input 
            |>  Array.map snd 
        let Pie = Chart.Pie (
            values = values,
            Labels = legendvalues
        )
        Pie 
    createChart
    //collecten der proteomIQon parameter

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
        let directoryPath = (String.concat "" [| "./arc/runs" ;directoryName|])
        //callt function with pipeline parameter, which is the equivalent to sndArg
        let searchpattern  = matchingDirecoryPath pipeline 
        //edit fasta file  
        let FastaArrayLength = readFasta fasta 
        let lengthParameter = FastaArrayLength |> Array.length 
        //called searchFile function with directroypath and searchpattern 
        let allData  = searchFiles directoryPath searchpattern
        let paramsArray =
            allData 
            |> fun x ->  matchingForFiles pipeline x
        //gives charts back
        let charts =
            paramsArray
            |> fun y-> dataPeptideEvidenceClass y lengthParameter
        let execution = 
            charts |>  createChart

        //layout
        let layout = 
            let axsisLayout () =
                LinearAxis.init (
                    ShowLine = true,
                    Ticks = StyleParam.TickOptions.Inside,
                    ZeroLine = false,
                    TickLabelStep = 1,
                    ShowTickLabels = true,
                    Mirror = StyleParam.Mirror.All,
                    TickFont = (Font.init (Size = 18))      
                )
            let majorLayout =    
                    Layout.init (
                        Title.init(
                                Text="<b>Protein identification plot: identified proteins in relation to parts of the <i>Chlamydomonas reinhardtii<i> proteome<b>", 
                                Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 30, Color = Color.fromString "Black")), 
                                XAnchor = StyleParam.XAnchorPosition.Center,
                                AutoMargin = false
                            ),
                            Font = Font.init (Size = 25)
                        )
                    |> Layout.setLinearAxis ((StyleParam.SubPlotId.XAxis 1), (axsisLayout ()))
                    |> Layout.setLinearAxis ((StyleParam.SubPlotId.YAxis 1), (axsisLayout ()))

            let traceLayout = 
                    [Trace2D.initScatter(
                        Trace2DStyle.Scatter(Marker = Marker.init (AutoColorScale = true)))]
                
            let templateYeast = Template.init (majorLayout, traceLayout)
            templateYeast
        
        //styling options for chart
        let stylingChart = 
            execution
                    |> Chart.withSize(1900,1600)
                    |> Chart.withMarginSize (250, 200, 130, 150)
                    |> Chart.withTemplate layout
                    |> fun x -> 
                        x |> Chart.saveHtml(String.concat "" [| "./arc/runs" ;directoryName;"/Results/ProteinIdentification/ProteinIdentification-plot" |])
        stylingChart
    finalChartHisto 
