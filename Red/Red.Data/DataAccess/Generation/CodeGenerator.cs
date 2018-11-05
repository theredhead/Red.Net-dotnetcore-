using System;
using System.Collections.Generic;
using System.IO;

namespace Red.Data.DataAccess.Generation
{
    public class CodeGenerator
    {
        private Dictionary<string, Stream> _files = new Dictionary<string, Stream>();

        public string RootNamespace { get; set; } = "Data";
        protected virtual string MakeDirName(string input) => input.Replace('.', '\\').Replace(' ', '_');

        protected void AddFile(string name, Stream stream)
        {
            var path = Path.Combine(MakeDirName(RootNamespace), name);
            if (_files.ContainsKey(name))
                throw new Exception("File already exists");

            _files[path] = stream;
        }

        public void Process(IDatabaseInfo database)
        {
            var name = $"{RootNamespace}/{database}.cs";
        }


        protected void GenerateDatabaseClass(IDatabaseInfo database)
        {
        }

        protected void GenerateTableClass(ITableInfo table)
        {
        }

        protected void GenerateRecordClass(ITableInfo table)
        {
        }


    }
}
