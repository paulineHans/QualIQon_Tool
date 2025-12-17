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


    let readMassSpecData (filePath: string) =

        let inReaderMS = new MzMLReader(filePath)
        let inReaderPeaks = new MzMLReader(filePath)
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
                            (inReaderPeaks.getSpecificPeak1DArraySequential ms.ID)
                    )
                | _ -> None
            )
            |> Seq.toArray 
            |> Array.collect (fun (rt,(xs, ys))-> 
                Array.zip xs ys
                |> Array.map (fun (mz,intensity)-> 
                    { RetentionTime = rt
                      mz = mz
                      intensity = intensity }))

        allSpectra |> getMassSpecData

    let createMS1Heatmap (massSpecData: pieces[]) =
        let create bandwidth (data: pieces[]) =            
            let halfBw = bandwidth / 1.0       
            data
            |> Seq.groupBy (fun x -> floor (x.RetentionTime / bandwidth)) 
            |> Seq.map (fun (k,values) ->                                 
                if k < 0. then
                    ((k * bandwidth) + halfBw, values)   
                else
                    ((k + 1.) * bandwidth) - halfBw, values)  
            |> Map.ofSeq

        let findMzBins (minMax: float * float) = 
            let min,max = minMax
            abs(ceil max - floor min) / 50.

        let minMaxAllSpectra  = 
            let mzValues =
                massSpecData |> Array.map (fun x -> x.mz)
            Array.min mzValues, Array.max mzValues

        let concatMzBins = minMaxAllSpectra |> findMzBins

        let createMzBins bandwidth (data: pieces[]) =            
            let halfBw = bandwidth / 1.0       
            data
            |> Seq.groupBy (fun x -> floor (x.mz / bandwidth)) 
            |> Seq.map (fun (k,values) ->                                 
                if k < 0. then
                    ((k * bandwidth) + halfBw, values)   
                else
                    ((k + 1.) * bandwidth) - halfBw, values)  
            |> Map.ofSeq

        let dataXIC  = 
            let binRT = 
                massSpecData
                |> create 1
                |> Map.toArray

            let binMz = 
                binRT 
                |> Array.map (fun (rt,y) -> 
                    let mzBins = 
                        y
                        |> Seq.toArray
                        |> createMzBins concatMzBins  
                        |> Map.toArray
                        |> Array.map (fun (mz,vals)-> 
                            mz,
                            vals
                            |> Seq.toArray
                            |> Array.map (fun x -> x.intensity)
                            |> Array.sum)
                    rt, mzBins)
            binMz

        let extractIntensity = 
            dataXIC
            |> Array.map (fun (_,y)-> 
                y |> Array.map snd)

        let chart = 
            Chart.Heatmap (zData = extractIntensity)
        chart

    let filesToMassSpectrum (directoryName: string) =
        let allData =
            System.IO.Directory.GetFiles (directoryName, "*.mzML")
        allData
        |> Array.map readMassSpecData
        |> Array.collect id
        |> createMS1Heatmap directoryName
























































    let filesToMassSpectrum (directoryPath: string) =

        let allData  = System.IO.Directory.GetFiles (directoryPath, "*.mzML")

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
                                    (inReaderPeaks.getSpecificPeak1DArraySequential ms.ID)
                            )
                        | _ -> None
                    )
                    |> Seq.toArray 
                    |> Array.map (fun (rt,(xs, ys))-> 
                        Array.zip xs ys
                        |> Array.map (fun (x,y)-> rt, (x,y))
                    )

                let massSpecData = allSpectra |> getMassSpecData
                massSpecData
            )
        exe
    

    

            //bins for RetentionTime
    let calc = 
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
                anwenden 
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

