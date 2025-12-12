namespace QualIQon 

open System
open ProteomIQon
open Plotly.NET
open MzIO.IO.MzML
open MzIO.Processing
open MzIO.Model

module MS1Map = 
        
        
    type pieces = {
    RetentionTime:  float;
    mz:             float; 
    intensity:      float 
}


    let customCulture: Globalization.CultureInfo = 
        System.Threading.Thread.CurrentThread.CurrentCulture.Clone() :?> Globalization.CultureInfo
        customCulture.NumberFormat.NumberDecimalSeparator <- "."
        System.Threading.Thread.CurrentThread.CurrentCulture <- customCulture


    let filesToMassSpectrum (directoryName: string) =
        let directorypath = (String.concat "" [| "./arc/runs"; directoryName; "/mzml" |])
        let allData  = System.IO.Directory.GetFiles (directorypath, "*.mzML")
        let createResultFolderMis = 
            System.IO.Directory.CreateDirectory ((String.concat "" [|"./arc/runs"; directoryName; "/Results/MS1map"|]))
        let exe = 
            allData
            |> Array.map (fun x -> 
            let inReaderMS = new MzMLReader(x)
            let inReaderPeaks = new MzMLReader(x)
            let inRunID  = Core.MzIO.Reader.getDefaultRunID inReaderMS 
            let allSpectra = inReaderMS.ReadMassSpectra inRunID
            
            let getMassSpecData (x : Collections.Generic.IEnumerable<MassSpectrum>) = 
                x
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
                |> Seq.toArray 
                |> Array.map (fun (rt,(xs, ys))-> 
                    Array.zip xs ys
                        |> Array.map (fun (x,y)-> 
                            rt, (x,y)
                        )
                    )

            //origin data
            let massSpecData = allSpectra |> getMassSpecData 

            //bins for RetentionTime
            let create bandwidth (data: pieces[]) =            
                let halfBw = bandwidth / 1.0       
                data
                |> Seq.groupBy (fun x -> floor (x.RetentionTime / bandwidth)) 
                |> Seq.map (fun (k,values) ->                                 
                    if k < 0. then
                        ((k  * bandwidth) + halfBw, values)   
                    else
                        ((k + 1.) * bandwidth) - halfBw, values)  
                |> Map.ofSeq

            //bins for inner m/Z values
            let findMzBins (minMax: float * float) = 
                let min,max = minMax
                abs(ceil max - floor min) / 50.

            //collect all m/z -> Min & Max for Binning 
            let minMaxAllSpectra  = 
                let data = 
                    massSpecData 
                    |> Array.map (fun x -> 
                        x
                        |> Array.map (fun (x,y)->  
                            y
                            |> fst
                        )
                    )
                let min = 
                    data 
                    |> Array.map(fun x -> 
                        x
                        |> Array.min)
                    |> Array.min
                    
                let max = 
                    data 
                    |> Array.map(fun x -> 
                        x
                        |> Array.max)
                    |> Array.max
                min, max 
            let concatMzBins = minMaxAllSpectra |> findMzBins

            let createMzBins bandwidth (data: pieces[]) =            
                let halfBw = bandwidth / 1.0       
                data
                |> Seq.groupBy (fun x -> floor (x.mz / bandwidth)) 
                |> Seq.map (fun (k,values) ->                                 
                    if k < 0. then
                        ((k  * bandwidth) + halfBw, values)   
                    else
                        ((k + 1.) * bandwidth) - halfBw, values)  
                |> Map.ofSeq

            let dataXIC  = 
                let accsessParameters = 
                    massSpecData 
                    |> Array.collect (fun x -> 
                        x
                        |> Array.map (fun (y ,(z,a))-> 
                            {RetentionTime = y;
                            mz = z;
                            intensity = a}))
                //anwendung der binning funktionen
                let binRT = 
                    accsessParameters
                    |> create 1
                    |> Map.toArray
                let binMz = 
                    binRT 
                    |> Array.map (fun (x,y) -> 
                        let muhaha = 
                            y
                            |> Seq.toArray
                            |> createMzBins concatMzBins  
                            |> Map.toArray
                            |> Array.map (fun (x,y)-> 
                                //Berechnung der SUmme der Intensitäten pro m/Z bin 
                                let calcSumIntensity = 
                                    y
                                    |> Seq.toArray
                                    |> Array.map (fun x -> x.intensity)
                                    |> Array.sum
                                x, calcSumIntensity)
                        x, muhaha
                    )
                binMz

            let Heatmap = 
                let extractIntensity = 
                    dataXIC
                    |> Array.map (fun x -> 
                        x
                        |> fun (x,y)-> 
                            y
                            |> Array.map (fun x -> snd x))  
                let chart = 
                    Chart.Heatmap (zData = extractIntensity)
                chart 
            Heatmap |> Chart.saveHtml (String.concat "" [| "./arc/runs"; directoryName; "/Results/MS1map/MS1map" |])
        )
        exe
    