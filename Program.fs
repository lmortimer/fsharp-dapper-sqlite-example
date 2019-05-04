open Database

[<EntryPoint>]
let main argv =
    
    Queries.Schema.CreateTables
    |> Async.RunSynchronously
    |> ignore

    // create a few bird records
    Queries.Bird.New "Kereru" "Wood Pigeon" |> Async.RunSynchronously |> ignore
    Queries.Bird.New "Kea" None |> Async.RunSynchronously |> ignore
    Queries.Bird.New "Doose" None |> Async.RunSynchronously |> ignore

    // display all birds
    printfn "== GetAll"
    Queries.Bird.GetAll()
    |> Async.RunSynchronously
    |> Seq.iter (printfn "%A")

    // update the kea record
    Queries.Bird.UpdateAliasByName "Kea" "Mountain Jester" |> Async.RunSynchronously |> ignore

    // make sure we can see the update
    match Queries.Bird.GetSingleByName "Kea" |> Async.RunSynchronously with
    | Some(bird) -> printfn "Kea alias is now %s" bird.Alias.Value
    | None -> printfn "Kea record does not exist"

    // delete an un-needed record
    Queries.Bird.DeleteByName "Doose" |> Async.RunSynchronously |> ignore

    // confirm it no longer exists
    match Queries.Bird.GetSingleByName "Doose" |> Async.RunSynchronously with
    | Some(bird) -> printfn "%A" bird
    | None -> printfn "Doose record does not exist"

    0
