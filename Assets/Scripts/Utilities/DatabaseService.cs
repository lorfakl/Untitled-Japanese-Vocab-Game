using System.Collections;
using System.Collections.Generic;
using SqlKata.Execution;
using System.Data;
using UnityEngine;
using SqlKata.Compilers;
using Microsoft.Data.Sqlite;

namespace Utilities.DatabaseServices
{
    public static class DatabaseService
    {
        // Start is called before the first frame update
        public static QueryFactory Connect(string connectString)
        {
            return new QueryFactory(new SqliteConnection(connectString), new SqliteCompiler());
        }

        
        // Update is called once per frame
        
    }
}

