// adapted from https://github.com/AlexTroshkin/fsharp-dapper/blob/master/README.md#define-your-own-query-builders--dataaccess--dbfs-

namespace Database

    open Microsoft.Data.Sqlite
    open FSharp.Data.Dapper

    module Connection =
        let private mkConnectionString (dataSource : string) =
            sprintf 
                "Data Source = %s; Mode = Memory; Cache = Shared;" 
                dataSource

        let private mkOnDiskConnectionString (dataSource: string) =
            sprintf
                "Data Source = %s;"
                dataSource

        let mkShared () = new SqliteConnection (mkConnectionString "MASTER")
        let mkOnDisk () = new SqliteConnection (mkOnDiskConnectionString "./example.db")

    module Types =

        [<CLIMutable>]
        type Bird = {
            Id : int64
            Name: string
            Alias: string option
        }

    module Queries = 
        let private connectionF () = Connection.SqliteConnection (Connection.mkOnDisk())

        let querySeqAsync<'R>    = querySeqAsync<'R> (connectionF)
        let querySingleAsync<'R> = querySingleOptionAsync<'R> (connectionF)

        module Schema = 
            let CreateTables = querySingleAsync<int> {
                script """
                    DROP TABLE IF EXISTS Bird;
                    CREATE TABLE Bird (
                        Id INTEGER PRIMARY KEY,
                        Name VARCHAR(255) NOT NULL,
                        Alias VARCHAR(255) NULL
                    );
                """
            }

        module Bird = 

            let New name alias = querySingleAsync<int> {
                script "INSERT INTO Bird (Name, Alias) VALUES (@Name, @Alias)"
                parameters (dict ["Name", box name; "Alias", box alias])
            }

            let GetSingleByName name = querySingleAsync<Types.Bird> {
                script "SELECT * FROM Bird WHERE Name = @Name LIMIT 1"
                parameters (dict ["Name", box name])
            }

            let GetAll name = querySeqAsync<Types.Bird> {
                script "SELECT * FROM Bird"
                parameters (dict ["Name", box name])
            }

            let UpdateAliasByName name alias = querySingleAsync<int> {
                script "UPDATE Bird SET Alias = @Alias WHERE Name = @Name"
                parameters (dict ["Alias", box alias; "Name", box name])
            }

            let DeleteByName name = querySingleAsync<int> {
                script "DELETE FROM Bird WHERE Name = @Name"
                parameters (dict ["Name", box name])
            }