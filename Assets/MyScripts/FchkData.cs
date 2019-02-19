using System.Collections.Generic;

public class FchkData{

    public string[] CommentLines { get; set; }
    public int NAtoms { get; set; }
    public int NAtomicOrbs { get; set; }
    public int NShells { get; set; }
    public int NTotalPrimitiveGTO { get; set; }
    public int[] AtomicNumbers { get; set; }
    public double[] NuclearCharges { get; set; }
    public double[,] AtomPositions { get; set; }

    public int[] ShellTypes { get; set; }
    public int[] NPrimitiveGTO { get; set; }
    public int[] ShellToAtomMap { get; set; }
    public double[] PrimitiveExponents { get; set; }
    public double[,] ShellPositions { get; set; }
    public double[] ContractionCoeffs { get; set; }
    public double[] AlphaMOCoeffs { get; set; }

    public static FchkData Load(string fchkFilePath)
    {
        string[] commentLines = new string[2];
        var iPropTable = new Dictionary<string, int[]>();
        var dPropTable = new Dictionary<string, double[]>();

        var fileInfo = new System.IO.FileInfo(fchkFilePath);
        string line;
        using (var sr = new System.IO.StreamReader(fileInfo.OpenRead(), System.Text.Encoding.UTF8))
        {
            commentLines[0] = sr.ReadLine();
            commentLines[1] = sr.ReadLine();

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line == "") { continue; }

                string title = line.Substring(0, 40).Trim();
                char dtype_symbol = line[43];
                bool is_array = line.Substring(47, 2) == "N=";
                string value = line.Substring(49);

                if (is_array)
                {
                    int ndata = int.Parse(value);
                    int j = 0;
                    switch (dtype_symbol)
                    {
                        case 'I':
                            iPropTable[title] = new int[ndata];
                            for (int i = 0; i < ndata; ++i, j = (j + 1) % 6){
                                if (j == 0) { line = sr.ReadLine(); }
                                iPropTable[title][i] = int.Parse(line.Substring(12 * j, 12));
                            }
                            break;
                        case 'R':
                            dPropTable[title] = new double[ndata];
                            for (int i = 0; i < ndata; ++i, j = (j + 1) % 5){
                                if (j == 0) { line = sr.ReadLine(); }
                                dPropTable[title][i] = double.Parse(line.Substring(16 * j, 16));
                            }
                            break;
                        case 'C':
                            for (int i = 0; i < (ndata - 1) / 6 + 1; ++i) { sr.ReadLine(); }
                            break;
                        case 'L':
                            for (int i = 0; i < (ndata - 1) / 72 + 1; ++i) { sr.ReadLine(); }
                            break;
                        case 'H':
                            for (int i = 0; i < (ndata - 1) / 9 + 1; ++i) { sr.ReadLine(); }
                            break;
                    }
                }
                else
                {
                    switch (dtype_symbol)
                    {
                        case 'I':
                            iPropTable[title] = new int[] { int.Parse(value) };
                            break;
                        case 'R':
                            dPropTable[title] = new double[] { double.Parse(value) };
                            break;
                    }
                }
            }
        } // close StreamReader

        var fchkData = new FchkData{
            CommentLines = commentLines
        };

        if(iPropTable.ContainsKey("Number of atoms")){
            fchkData.NAtoms = iPropTable["Number of atoms"][0];
        }
        if(iPropTable.ContainsKey("Number of basis functions")){
            fchkData.NAtomicOrbs = iPropTable["Number of basis functions"][0];
        }
        if(iPropTable.ContainsKey("Number of contracted shells")){
            fchkData.NShells = iPropTable["Number of contracted shells"][0];
        }
        if(iPropTable.ContainsKey("Number of primitive shells")){
            fchkData.NTotalPrimitiveGTO = iPropTable["Number of primitive shells"][0];
        }

        if(iPropTable.ContainsKey("Atomic numbers")){
            fchkData.AtomicNumbers = iPropTable["Atomic numbers"];
        }
        if(dPropTable.ContainsKey("Nuclear charges")){
            fchkData.NuclearCharges = dPropTable["Nuclear charges"];
        }
        if(dPropTable.ContainsKey("Current cartesian coordinates")){
            var positions = dPropTable["Current cartesian coordinates"];
            var nAtom = positions.Length / 3;
            fchkData.AtomPositions = new double[nAtom, 3];
            for (int i = 0; i < positions.Length; ++i){
                fchkData.AtomPositions[i/3,i%3] = positions[i];
            }
        }

        if(iPropTable.ContainsKey("Shell types")){
            fchkData.ShellTypes = iPropTable["Shell types"];
        }
        if(iPropTable.ContainsKey("Number of primitives per shell")){
            fchkData.NPrimitiveGTO = iPropTable["Number of primitives per shell"];
        }
        if(iPropTable.ContainsKey("Shell to atom map")){
            fchkData.ShellToAtomMap = iPropTable["Shell to atom map"];
        }

        if(dPropTable.ContainsKey("Primitive exponents")){
            fchkData.PrimitiveExponents = dPropTable["Primitive exponents"];
        }
        if(dPropTable.ContainsKey("Coordinates of each shell")){
            var coordinates = dPropTable["Coordinates of each shell"];
            var nShell = coordinates.Length / 3;
            fchkData.ShellPositions = new double[nShell, 3];
            for (int i = 0; i < coordinates.Length; ++i)
            {
                fchkData.ShellPositions[i / 3, i % 3] = coordinates[i];
            }
        }
        if(dPropTable.ContainsKey("Contraction coefficients")){
            fchkData.ContractionCoeffs = dPropTable["Contraction coefficients"];
        }
        if (dPropTable.ContainsKey("Alpha MO coefficients")){
            fchkData.AlphaMOCoeffs = dPropTable["Alpha MO coefficients"];
        }


        return fchkData;
    }


    public IEnumerable<float> LoadCoroutine(string fchkFilePath, int interval = 10000){
        string[] commentLines = new string[2];
        var iPropTable = new Dictionary<string, int[]>();
        var dPropTable = new Dictionary<string, double[]>();

        var fileInfo = new System.IO.FileInfo(fchkFilePath);
        var fileLength = fileInfo.Length;
        long progress = 0;
        long next_yield = interval;

        string line;
        using (var sr = new System.IO.StreamReader(fileInfo.OpenRead(), System.Text.Encoding.UTF8))
        {
            commentLines[0] = sr.ReadLine();
            commentLines[1] = sr.ReadLine();

            progress += commentLines[0].Length + commentLines[1].Length + 2;

            while (!sr.EndOfStream)
            {
                if (progress > next_yield){
                    next_yield += interval;
                    yield return (float)progress / (float)fileLength;
                }

                line = sr.ReadLine();
                progress += line.Length + 1;
                if (line == "") { continue; }

                string title = line.Substring(0, 40).Trim();
                char dtype_symbol = line[43];
                bool is_array = line.Substring(47, 2) == "N=";
                string value = line.Substring(49);

                if (is_array)
                {
                    int ndata = int.Parse(value);
                    int j = 0;
                    switch (dtype_symbol)
                    {
                        case 'I':
                            iPropTable[title] = new int[ndata];
                            for (int i = 0; i < ndata; ++i, j = (j + 1) % 6){
                                if (j == 0) {
                                    line = sr.ReadLine();
                                    progress += line.Length + 1;
                                    if (progress > next_yield){
                                        next_yield += interval;
                                        yield return (float)progress / (float)fileLength;
                                    }
                                }
                                iPropTable[title][i] = int.Parse(line.Substring(12 * j, 12));
                            }
                            break;
                        case 'R':
                            dPropTable[title] = new double[ndata];
                            for (int i = 0; i < ndata; ++i, j = (j + 1) % 5){
                                if (j == 0) {
                                    line = sr.ReadLine();
                                    progress += line.Length + 1;
                                    if (progress > next_yield){
                                        next_yield += interval;
                                        yield return (float)progress / (float)fileLength;
                                    }
                                }
                                dPropTable[title][i] = double.Parse(line.Substring(16 * j, 16));
                            }
                            break;
                        case 'C':
                            for (int i = 0; i < (ndata - 1) / 6 + 1; ++i) {
                                line = sr.ReadLine();
                                progress += line.Length + 1;
                                if (progress > next_yield){
                                    next_yield += interval;
                                    yield return (float)progress / (float)fileLength;
                                }
                            }
                            break;
                        case 'L':
                            for (int i = 0; i < (ndata - 1) / 72 + 1; ++i) {
                                line = sr.ReadLine();
                                progress += line.Length + 1;
                                if (progress > next_yield){
                                    next_yield += interval;
                                    yield return (float)progress / (float)fileLength;
                                }
                            }
                            break;
                        case 'H':
                            for (int i = 0; i < (ndata - 1) / 9 + 1; ++i) {
                                line = sr.ReadLine();
                                progress += line.Length + 1;
                                if (progress > next_yield){
                                    next_yield += interval;
                                    yield return (float)progress / (float)fileLength;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (dtype_symbol)
                    {
                        case 'I':
                            iPropTable[title] = new int[] { int.Parse(value) };
                            break;
                        case 'R':
                            dPropTable[title] = new double[] { double.Parse(value) };
                            break;
                    }
                }
            }
        } // close StreamReader

        CommentLines = commentLines;

        if (iPropTable.ContainsKey("Number of atoms")){
            NAtoms = iPropTable["Number of atoms"][0];
        }
        if (iPropTable.ContainsKey("Number of basis functions")){
            NAtomicOrbs = iPropTable["Number of basis functions"][0];
        }
        if (iPropTable.ContainsKey("Number of contracted shells")){
            NShells = iPropTable["Number of contracted shells"][0];
        }
        if (iPropTable.ContainsKey("Number of primitive shells")){
            NTotalPrimitiveGTO = iPropTable["Number of primitive shells"][0];
        }

        if (iPropTable.ContainsKey("Atomic numbers")){
            AtomicNumbers = iPropTable["Atomic numbers"];
        }
        if (dPropTable.ContainsKey("Nuclear charges")){
            NuclearCharges = dPropTable["Nuclear charges"];
        }
        if (dPropTable.ContainsKey("Current cartesian coordinates")){
            var positions = dPropTable["Current cartesian coordinates"];
            var nAtom = positions.Length / 3;
            AtomPositions = new double[nAtom, 3];
            for (int i = 0; i < positions.Length; ++i){
                AtomPositions[i / 3, i % 3] = positions[i];
            }
        }

        if (iPropTable.ContainsKey("Shell types")){
            ShellTypes = iPropTable["Shell types"];
        }
        if (iPropTable.ContainsKey("Number of primitives per shell")){
            NPrimitiveGTO = iPropTable["Number of primitives per shell"];
        }
        if (iPropTable.ContainsKey("Shell to atom map")){
            ShellToAtomMap = iPropTable["Shell to atom map"];
        }

        if (dPropTable.ContainsKey("Primitive exponents")){
            PrimitiveExponents = dPropTable["Primitive exponents"];
        }
        if (dPropTable.ContainsKey("Coordinates of each shell")){
            var coordinates = dPropTable["Coordinates of each shell"];
            var nShell = coordinates.Length / 3;
            ShellPositions = new double[nShell, 3];
            for (int i = 0; i < coordinates.Length; ++i){
                ShellPositions[i / 3, i % 3] = coordinates[i];
            }
        }
        if (dPropTable.ContainsKey("Contraction coefficients")){
            ContractionCoeffs = dPropTable["Contraction coefficients"];
        }
        if (dPropTable.ContainsKey("Alpha MO coefficients")){
            AlphaMOCoeffs = dPropTable["Alpha MO coefficients"];
        }

        yield return 1f;
    }

    public static void Main(){
        var fchk = FchkData.Load("h2o.fchk");
        System.Console.WriteLine(fchk.NAtoms);
        System.Console.WriteLine(fchk.NAtomicOrbs);
    }
}
