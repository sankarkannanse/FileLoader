using FileHelpers;
using FileLoader.Loader.Implementation;
using FileLoader.Loader.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FileLoader
{
    class Program
    {
        public static string MappingCSV = @"C:\Users\ksank\Downloads\loadfile.csv";
        public static string MappingFile = @"C:\Users\ksank\source\repos\FileLoader\FileLoader\MetaDoc.xml";
        static List<string> FieldNames = new List<string>();

        static void Main(string[] args)
        {
            //string row = "sank sank sang kuma India India Pakis China";
            //Console.WriteLine(row);

            Console.WriteLine(ReadZipFile(@"E:\Fld.zip"));

            string inputFilePath = ReadZipFile(@"E:\Fld.zip");
        }
        //static string destinationFolder = @"C:\temp";
        static string ReadZipFile(string path)
        {
            String sPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!sPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                sPath += Path.DirectorySeparatorChar;
            string destinationPathName = "";
            string zipPath = path;
            var file = System.IO.Compression.ZipFile.OpenRead(zipPath)
              .Entries.Where(x => x.Name.EndsWith(".txt",
                                           StringComparison.InvariantCulture))
              .FirstOrDefault();
            if (file != null)
            {
                destinationPathName = sPath + file.Name; ;
                if (File.Exists(destinationPathName))
                {
                    File.Delete(destinationPathName);

                }
                file.ExtractToFile(destinationPathName);
                return destinationPathName;
            }
            else
            {
                throw new Exception("File not exist");
            }



        }

        public static List<Field> GetFields()
        {
            List<Field> fields = new List<Field>();
            var xDoc = new XmlDocument();
            //  if (Utils.ToBoolean(ConfigSection.LoadingMappingFile)) ConvertCSVToXML();
            if (Utils.ToBoolean("Y")) ConvertCSVToXML();

            xDoc.Load(@"C:\Users\ksank\source\repos\FileLoader\FileLoader\MetaDoc.xml");
            xDoc.SelectNodes("/FileMap/Field").Cast<XmlNode>().ToList().ForEach(fieldNode =>
            {
                fields.Add(new Field
                {
                    FieldName = fieldNode.Attributes["FieldName"].Value.Trim(),
                    FieldLength = Convert.ToInt32(fieldNode.Attributes["FieldLength"].Value.Trim()),
                    Offset = Convert.ToInt32(fieldNode.Attributes["Offset"].Value.Trim()),
                    // Encrypt = Utils.ToBoolean(fieldNode.Attributes["Encrypt"].Value.Trim()),
                    CryptId = fieldNode.Attributes["CryptId"].Value.Trim(),
                    Subscript1 = fieldNode.Attributes["Subscript1"].Value.Trim(),
                    Subscript2 = fieldNode.Attributes["Subscript2"].Value.Trim(),
                    Subscript3 = fieldNode.Attributes["Subscript3"].Value.Trim(),

                });

            });
            return fields;
        }

        private static void ConvertCSVToXML()
        {
            string[] source = File.ReadAllLines(MappingCSV);
            string cryptId = string.Empty;
            var fieldsToBeEncrypted = new Dictionary<string, string>(){
                {"METADOC-MLSET-TYP-DESC", "GenericText_Internal"},
                {"METADOC-PVC-CARR-ID", "GenericText_Internal"},
                {"METADOC-DOC-ID", "GenericText_Internal"}
            };

            XElement xDoc = new XElement("FileMap",
                from str in source
                let fields = str.Split(',')
                where fields[0] != "88" && fields[5].ToLower() != "group"
                select new XElement("Field",
                    new XAttribute("CobolLevel", fields[0]),
                    new XAttribute("FieldName", fields[1].Trim()),
                    new XAttribute("Subscript1", fields[2]),
                    new XAttribute("Subscript2", fields[3]),
                    new XAttribute("Subscript3", fields[4]),
                    new XAttribute("FieldDefinition", fields[5]),
                    new XAttribute("NumericStructure", fields[6]),
                    new XAttribute("Offset", fields[7]),
                    new XAttribute("ByteLength", fields[8]),
                    new XAttribute("FieldLength", fields[9]),
                    new XAttribute("IntegerPart", fields[10]),
                    new XAttribute("DecimalPart", fields[11]),
                    new XAttribute("RedefinedField", fields[12]),
                    new XAttribute("UserDefinedField", fields[13]),
                    new XAttribute("Encrypt", fieldsToBeEncrypted.ContainsKey(fields[1].Trim()) ? "Y" : "N"),
                    new XAttribute("CryptId", fieldsToBeEncrypted.TryGetValue(fields[1].Trim(), out cryptId) ? cryptId : string.Empty)
                )
            );
            xDoc.Save(MappingFile);
        }

        private static void ProcessDuplicates(string filepathcsv)
        {
            string[] source = File.ReadAllLines(filepathcsv);
            string cryptId = string.Empty;
            var fieldsToBeEncrypted = new Dictionary<string, string>(){
                {"METADOC-MLSET-TYP-DESC", "GenericText_Internal"},
                {"METADOC-PVC-CARR-ID", "GenericText_Internal"},
                {"METADOC-DOC-ID", "GenericText_Internal"}
            };

            List<CSVObject> xDoc = new List<CSVObject>();
            xDoc = (from str in source
                    let fields = str.Split(',')
                    where fields[0] != "88" && fields[5].ToLower() != "group"
                    select new CSVObject
                    {
                        CobolLevel = fields[0],
                        FieldName = fields[1].Trim(),
                        Subscript1 = fields[2],
                        Subscript2 = fields[3],
                        Subscript3 = fields[4],
                        FieldDefinition = fields[5],
                        NumericStructure = fields[6],
                        Offset = fields[7],
                        ByteLength = fields[8],
                        FieldLength = fields[9],
                        IntegerPart = fields[10],
                        DecimalPart = fields[11],
                        RedefinedField = fields[12],
                        UserDefinedField = fields[13],
                        Encrypt = fieldsToBeEncrypted.ContainsKey(fields[1].Trim()) ? "Y" : "N",
                        CryptId = fieldsToBeEncrypted.TryGetValue(fields[1].Trim(), out cryptId) ? cryptId : string.Empty

                    }).ToList();

            foreach (CSVObject record in xDoc)
            {
                string Subscript1 = string.Empty, Subscript2 = string.Empty, Subscript3 = string.Empty;
                Subscript1 = record.Subscript1;
                Subscript1 = Subscript1 == "0" ? "" : "-" + Subscript1;
                Subscript2 = record.Subscript2;
                Subscript2 = Subscript2 == "0" ? "" : "-" + Subscript2;
                Subscript3 = record.Subscript3;
                Subscript3 = Subscript3 == "0" ? "" : "-" + Subscript3;
                string Fieldname = record.FieldName;
                if (xDoc.Count(a => a.FieldName == record.FieldName) > 1 || xDoc.Any(a => a.FieldName.Contains(Fieldname)))
                {

                    record.FieldName = record.FieldName + Subscript1;


                    if (xDoc.Count(a => a.FieldName == record.FieldName) > 1 || xDoc.Any(a => a.FieldName.Contains(Fieldname)))
                    {
                        record.FieldName = record.FieldName + Subscript2;


                        if (xDoc.Count(a => a.FieldName == record.FieldName) > 1 || xDoc.Any(a => a.FieldName.Contains(Fieldname)))
                        {
                            record.FieldName = record.FieldName + Subscript3;
                        }
                    }

                }
            }

            XElement xDocFile = new XElement("FileMap",
               from str in xDoc


               select new XElement("Field",
                   new XAttribute("CobolLevel", str.CobolLevel),
                   new XAttribute("FieldName", str.FieldName.Trim()),
                   new XAttribute("Subscript1", str.Subscript1),
                   new XAttribute("Subscript2", str.Subscript2),
                   new XAttribute("Subscript3", str.Subscript3),
                   new XAttribute("FieldDefinition", str.FieldDefinition),
                   new XAttribute("NumericStructure", str.NumericStructure),
                   new XAttribute("Offset", str.Offset),
                   new XAttribute("ByteLength", str.ByteLength),
                   new XAttribute("FieldLength", str.FieldLength),
                   new XAttribute("IntegerPart", str.IntegerPart),
                   new XAttribute("DecimalPart", str.DecimalPart),
                   new XAttribute("RedefinedField", str.RedefinedField),
                   new XAttribute("UserDefinedField", str.UserDefinedField),
                   new XAttribute("Encrypt", str.Encrypt),
                   new XAttribute("CryptId", str.CryptId)
               )
           );
            xDocFile.Save(MappingFile);





        }

        public static List<List<Field>> ParseFile(string inputFile)
        {

            List<Field> fields = GetFields();

            List<List<Field>> records = new List<List<Field>>();
            using (StreamReader reader = new StreamReader(inputFile))
            {
                string line = reader.ReadLine();

                while (line != null)
                {

                    List<Field> record = new List<Field>();
                    foreach (var field in fields)
                    {
                        // To Do: Identify the fileds that are getting chocked.
                        // Adding a conditional statement to avoid System.ArgumentOutOfRangeException.
                        if (line.Length > field.Offset + field.FieldLength)
                        {
                            Field fileField = new Field();
                            fileField.Offset = field.Offset;
                            fileField.FieldLength = field.FieldLength;
                            fileField.CryptId = field.CryptId;
                            string value = line.Substring(field.Offset, field.FieldLength).Trim();
                            fileField.Value = value;
                            fileField.FieldName = field.FieldName;
                            record.Add(fileField);
                        }
                    }
                    var rcd = RemoveDuplicate(fields, record);
                    records.Add(record);

                    line = reader.ReadLine();
                }
            }
            return records;
        }

        public static List<Field> RemoveDuplicate(List<Field> fields, List<Field> record)
        {
            if (record.Count > 0)
            {

                string Subscript1 = string.Empty, Subscript2 = string.Empty, Subscript3 = string.Empty;
                Subscript1 = record.FirstOrDefault().Subscript1;
                Subscript1 = Subscript1 == "0" ? "" : "-" + Subscript1;
                Subscript2 = record.FirstOrDefault().Subscript2;
                Subscript2 = Subscript2 == "0" ? "" : "-" + Subscript2;
                Subscript3 = record.FirstOrDefault().Subscript3;
                Subscript3 = Subscript3 == "0" ? "" : "-" + Subscript3;

                if (fields.Count(a => a.FieldName == record.FirstOrDefault().FieldName) > 1 || FieldNames.Any(a => a == record.FirstOrDefault().FieldName))
                {

                    record.FirstOrDefault().FieldName = record.FirstOrDefault().FieldName + Subscript1;


                    if (fields.Count(a => a.FieldName == record.FirstOrDefault().FieldName) > 1 || FieldNames.Any(a => a == record.FirstOrDefault().FieldName))
                    {
                        record.FirstOrDefault().FieldName = record.FirstOrDefault().FieldName + Subscript2;


                        if (fields.Count(a => a.FieldName == record.FirstOrDefault().FieldName) > 1 || FieldNames.Any(a => a == record.FirstOrDefault().FieldName))
                        {
                            record.FirstOrDefault().FieldName = record.FirstOrDefault().FieldName + Subscript3;
                        }
                    }

                }
                //foreach (var column in record)
                //{
                //    var dataColumn = record.Where(a => a.FieldName == column.FieldName).FirstOrDefault();
                //    var duplicateDataCount = record.Where(a => a.Value == dataColumn.Value).Count();
                //    Subscript1 = fields.Where(a => a.FieldName == column.FieldName).FirstOrDefault().Subscript1;
                //    Subscript1 = Subscript1 == "0" ? "" : "-" + Subscript1;
                //    Subscript2 = fields.Where(a => a.FieldName == column.FieldName).FirstOrDefault().Subscript2;
                //    Subscript2 = Subscript2 == "0" ? "" : "-" + Subscript2;
                //    Subscript3 = fields.Where(a => a.FieldName == column.FieldName).FirstOrDefault().Subscript3;
                //    Subscript3 = Subscript3 == "0" ? "" : "-" + Subscript3;
                //    if (record.Where(a => a.Value == dataColumn.Value).Count() > 1)
                //    {
                //        dataColumn.Value = dataColumn.Value + Subscript1;


                //        if (record.Where(a => a.Value.Trim() == dataColumn.Value).Count() > 1)
                //        {
                //            dataColumn.Value = dataColumn.Value +Subscript2;


                //            if (record.Where(a => a.Value == dataColumn.Value).Count() > 1) 
                //            {
                //                dataColumn.Value = dataColumn.Value +  Subscript3;
                //            }
                //        }

                //    }
                //}


                return record;
            }
            else
            {
                return null;
            }

        }
    }

    [DelimitedRecord(",")]
    public class CSVObject
    {
        public string CobolLevel { get; set; }

        public string FieldName { get; set; }
        public string Subscript1 { get; set; }
        public string Subscript2 { get; set; }
        public string Subscript3 { get; set; }
        public string FieldDefinition { get; set; }
        public string NumericStructure { get; set; }
        public string Offset { get; set; }
        public string ByteLength { get; set; }
        public string FieldLength { get; set; }
        public string IntegerPart { get; set; }
        public string DecimalPart { get; set; }
        public string RedefinedField { get; set; }
        public string UserDefinedField { get; set; }
        public string Encrypt { get; set; }
        public string CryptId { get; set; }
    }

    public class Field
    {
        public string FieldName { get; set; }
        public string Subscript1 { get; set; }

        public string Subscript2 { get; set; }
        public string Subscript3 { get; set; }
        public int Offset { get; set; }
        public int FieldLength { get; set; }
        public string Value { get; set; }
        public Boolean Encrypt { get; set; }
        public string CryptId { get; set; }
    }




}
