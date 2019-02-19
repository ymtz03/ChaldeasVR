using System.Collections.Generic;
using System;

public class CubeGenerator
{
    
    class BasisSet
    {
        int NAtom { get; set; }
        double[,] AtomPositions { get; set; } // (nAtom,3), arg
        int[] MaxAngularMomentum { get; set; } // Length = nAtom

        int NShell { get; set; }
        int[] ShellTypes { get; set; }         // Length = nShell, arg
        int[] NPrimitiveGTO { get; set; }      // Length = nShell, arg
        int[] ShellToAtomMap { get; set; }     // Length = nShell, arg
        int[] ShellToExponentMap { get; set; } // Length = nShell

        public readonly int NExponent;
        double[] PrimitiveExponents { get; set; } // Length = nExponents, arg
        double[] ContractionCoeffs { get; set; }  // Length = nExponents, arg
        double[] NormalizedCoeffs { get; set; }   // Length = nExponents
        int[] ExponentToAtomMap { get; set; }     // Length = nExponents
        int[] ExponentToShellMap { get; set; }    // Length = nExponents

        public readonly int NAtomicOrb;

        double[,] DisplaceFromAtoms { get; set; } // (nAtom,7) = x,y,z,x2,y2,z2,r2

        public BasisSet(
            double[,] atomPositions,
            int[] shellTypes, int[] nPrimitiveGTO, int[] shellToAtomMap,
            double[] primitiveExponents,
            double[] contractionCoeffs
        ){
            AtomPositions = atomPositions;
            NAtom = AtomPositions.GetLength(0);
            MaxAngularMomentum = new int[NAtom];

            ShellTypes = shellTypes;
            NPrimitiveGTO = nPrimitiveGTO;
            ShellToAtomMap = shellToAtomMap;
            NShell = ShellTypes.Length;
            ShellToExponentMap = new int[NShell];

            PrimitiveExponents = primitiveExponents;
            ContractionCoeffs = contractionCoeffs;
            NExponent = PrimitiveExponents.Length;
            NormalizedCoeffs = new double[NExponent];
            ExponentToAtomMap = new int[NExponent];
            ExponentToShellMap = new int[NExponent];

            DisplaceFromAtoms = new double[NAtom, 7];

            // init maps
            {
                var nAOMap = new System.Collections.Generic.Dictionary<int, int>{
                    {0,1},{1,3},{2,6},{-2,5},{3,10},{-3,7},{4,15},{-4,9},{5,21},{-5,11}
                };
                int iep = 0;
                for (int ish = 0; ish < NShell; ++ish){
                    var iatom = ShellToAtomMap[ish]-1;
                    ShellToExponentMap[ish] = iep;
                    for (int i = 0; i < NPrimitiveGTO[ish]; ++i, ++iep){
                        ExponentToShellMap[iep] = ish;
                        ExponentToAtomMap[iep] = iatom;
                    }
                    var angularMomentum = Math.Abs(ShellTypes[ish]);
                    MaxAngularMomentum[iatom] = Math.Max(MaxAngularMomentum[iatom], 
                                                                angularMomentum);
                    NAtomicOrb += nAOMap[ShellTypes[ish]];
                }
            }

            // calc NormalizedCoeffs
            var factor = new int[] { 1, 1, 3, 15, 105, 945 };
            for (int ish = 0; ish < NShell; ++ish){
                var primitiveGTONomalizeFactors = new double[NPrimitiveGTO[ish]];
                var angularMomentum = Math.Abs(ShellTypes[ish]);
                var f = factor[angularMomentum];

                for (int iep = 0; iep < NPrimitiveGTO[ish]; ++iep)
                {
                    int iPGTO = ShellToExponentMap[ish] + iep;
                    double a = PrimitiveExponents[iPGTO];

                    var imd1 = f / Math.Pow(4 * a, angularMomentum);
                    var imd2 = Math.PI / (2 * a);
                    imd2 *= Math.Sqrt(imd2);
                    primitiveGTONomalizeFactors[iep] = 1.0 / Math.Sqrt(imd1 * imd2);
                }

                double contractedGTONormalizeFactor = 0;
                for (int iep = 0; iep < NPrimitiveGTO[ish]; ++iep){
                    int iPGTO = ShellToExponentMap[ish] + iep;
                    var a_i = PrimitiveExponents[iPGTO];
                    var d_i = ContractionCoeffs[iPGTO];
                    var n_i = primitiveGTONomalizeFactors[iep];
                    double imd = 0;

                    for (int jep = 0; jep < NPrimitiveGTO[ish]; ++jep){
                        int jPGTO = ShellToExponentMap[ish] + jep;
                        var a_j = PrimitiveExponents[jPGTO];
                        var d_j = ContractionCoeffs[jPGTO];
                        var n_j = primitiveGTONomalizeFactors[jep];

                        var imd2 = Math.PI / (a_i + a_j);
                        imd2 *= Math.Sqrt(imd2);

                        imd += d_j * n_j / Math.Pow(2 * (a_i + a_j), angularMomentum) * imd2;
                    }

                    contractedGTONormalizeFactor += d_i * n_i * imd;
                }
                contractedGTONormalizeFactor = 1.0 / Math.Sqrt(contractedGTONormalizeFactor*f);

                for (int iep = 0; iep < NPrimitiveGTO[ish]; ++iep){
                    int iPGTO = ShellToExponentMap[ish] + iep;
                    NormalizedCoeffs[iPGTO] = ContractionCoeffs[iPGTO] 
                        * primitiveGTONomalizeFactors[iep] * contractedGTONormalizeFactor;
                }
            }
        }

        public double[] Eval(double[] pos)
        {
            for (int iatom = 0; iatom < NAtom; ++iatom){
                double r2 = 0;
                for (int iaxis = 0; iaxis < 3; ++iaxis){
                    var d = pos[iaxis] - AtomPositions[iatom, iaxis];
                    var d2 = d * d;
                    DisplaceFromAtoms[iatom, iaxis] = d;
                    DisplaceFromAtoms[iatom, iaxis+3] = d2;
                    r2 += d2;
                }
                DisplaceFromAtoms[iatom, 6] = r2;
            }

            var v = new double[NExponent];  // v_i = c_i * PGTO_i(r2)
            for (int iep = 0; iep < NExponent; ++iep){
                var r2 = DisplaceFromAtoms[ExponentToAtomMap[iep], 6];
                var c = NormalizedCoeffs[iep];
                var a = PrimitiveExponents[iep];
                v[iep] = c * Math.Exp(-a * r2);
            }

            var retval = new double[NAtomicOrb];
            int iao = 0;
            for (int ish = 0; ish < NShell; ++ish){
                double contractedValue = 0;
                for (int iep = 0; iep < NPrimitiveGTO[ish]; ++iep){
                    var iPGTO = ShellToExponentMap[ish] + iep;
                    contractedValue += v[iPGTO];
                }

                var iatom = ShellToAtomMap[ish]-1;
                var dx  = DisplaceFromAtoms[iatom, 0];
                var dy  = DisplaceFromAtoms[iatom, 1];
                var dz  = DisplaceFromAtoms[iatom, 2];
                var dx2 = DisplaceFromAtoms[iatom, 3];
                var dy2 = DisplaceFromAtoms[iatom, 4];
                var dz2 = DisplaceFromAtoms[iatom, 5];
                var r2 =  DisplaceFromAtoms[iatom, 6];

                switch(ShellTypes[ish]){
                    case 0:
                        retval[iao] = contractedValue;
                        iao += 1;
                        break;
                    case 1:
                        retval[iao    ] = contractedValue * dx;
                        retval[iao + 1] = contractedValue * dy;
                        retval[iao + 2] = contractedValue * dz;
                        iao += 3;
                        break;
                    case 2:
                        retval[iao    ] = contractedValue * dx2;
                        retval[iao + 1] = contractedValue * dy2;
                        retval[iao + 2] = contractedValue * dz2;
                        retval[iao + 3] = contractedValue * dx * dy * Math.Sqrt(3);
                        retval[iao + 4] = contractedValue * dx * dz * Math.Sqrt(3);
                        retval[iao + 5] = contractedValue * dy * dz * Math.Sqrt(3);
                        iao += 6;
                        break;
                    case -2:
                        retval[iao    ] = contractedValue * (3 * dz2 - r2) * 0.5;
                        retval[iao + 1] = contractedValue * dx * dz * Math.Sqrt(3);
                        retval[iao + 2] = contractedValue * dy * dz * Math.Sqrt(3);
                        retval[iao + 3] = contractedValue * (dx2 -  dy2) * Math.Sqrt(0.75);
                        retval[iao + 4] = contractedValue * dx * dy * Math.Sqrt(3);
                        iao += 5;
                        break;
                    case 3:
                        retval[iao    ] = contractedValue * dx2 * dx;
                        retval[iao + 1] = contractedValue * dy2 * dy;
                        retval[iao + 2] = contractedValue * dz2 * dz;
                        retval[iao + 3] = contractedValue * dx * dy2 * Math.Sqrt(5);
                        retval[iao + 4] = contractedValue * dx2 * dy * Math.Sqrt(5);
                        retval[iao + 5] = contractedValue * dx2 * dz * Math.Sqrt(5);
                        retval[iao + 6] = contractedValue * dx * dz2 * Math.Sqrt(5);
                        retval[iao + 7] = contractedValue * dy * dz2 * Math.Sqrt(5);
                        retval[iao + 8] = contractedValue * dy2 * dz * Math.Sqrt(5);
                        retval[iao + 9] = contractedValue * dx * dy * dz * Math.Sqrt(15);
                        iao += 10;
                        break;
                    case -3:
                        retval[iao    ] = contractedValue * dz * (5 * dz2 - 3 * r2) * 0.5;           // Z(ZZ-RR)
                        retval[iao + 1] = contractedValue * dx * (5 * dz2 - r2) * Math.Sqrt(0.375);  // X(ZZ-RR)
                        retval[iao + 2] = contractedValue * dy * (5 * dz2 - r2) * Math.Sqrt(0.375);  // Y(ZZ-RR)
                        retval[iao + 3] = contractedValue * dz * (dx2 - dy2) * Math.Sqrt(3.75);      // (XX-YY)Z
                        retval[iao + 4] = contractedValue * dx * dy * dz * Math.Sqrt(15);            // XYZ
                        retval[iao + 5] = contractedValue * dx * (dx2 - 3 * dy2) * Math.Sqrt(0.625); // X(XX-YY)
                        retval[iao + 6] = contractedValue * dy * (3 * dx2 - dy2) * Math.Sqrt(0.625); // Y(XX-YY)
                        iao += 7;
                        break;
                    case 4:
                        iao += 15;
                        break;
                    case -4:
                        iao += 9;
                        break;
                    case 5:
                        iao += 21;
                        break;
                    case -5:
                        iao += 11;
                        break;
                    case 6:
                        iao += 28;
                        break;
                    case -6:
                        iao += 13;
                        break;
                }
            }

            return retval;
        }
    }

    public static CubeData CubeGen(FchkData fchkData, int[] iMolecularOrbs, int nVoxel)
    {
        string[] commentLines = { "Foo", "Bar" };
        int nAtoms = fchkData.NAtoms;
        int[] atomicNumbers = fchkData.AtomicNumbers;
        double[] nuclearCharges = fchkData.NuclearCharges;
        double[,] atomPositions = fchkData.AtomPositions;

        int[] nVoxels = { nVoxel, nVoxel, nVoxel };
        double[] origin = new double[3];
        double[,] axes = new double[3, 3];
        GetGridParams(atomPositions, nVoxels, origin, axes);

        // THIS SECTION IS FOR DEBUG !! DELETE HERE LATER-----
        nVoxels = new int[] {60,60,60};
        //origin  = new double[] {-4.970736,   -4.970736,   -4.970736};
        origin  = new double[] {-7.157859,   -7.157859,   -7.157859};
        axes = new double[3,3];
        //axes[0,0] = axes[1,1] = axes[2,2] = 0.168500;
        axes[0,0] = axes[1,1] = axes[2,2] = 0.242639;
        
        // nVoxels = new int[] {120,120,120 };
        // origin = new double[] { -4.970736*2, -4.970736*2, -4.970736*2 };
        // axes = new double[3, 3];
        // axes[0, 0] = axes[1, 1] = axes[2, 2] = 0.168500;
        // ---------------------------------------------------

        double[,,,] data = CubeGenCore(
            atomPositions, fchkData.ShellTypes, fchkData.NPrimitiveGTO, fchkData.ShellToAtomMap,
            fchkData.PrimitiveExponents, fchkData.ContractionCoeffs,
            nVoxels, origin, axes,
            fchkData.AlphaMOCoeffs, iMolecularOrbs);

        int nVals = iMolecularOrbs.Length;
        int[] dsetIds = new int[nVals];
        for (int i = 0; i < nVals; ++i){
            dsetIds[i] = iMolecularOrbs[i] + 1;
        }

        var data_ = new List<double[,,]>();
        for (int im = 0; im < data.GetLength(3); ++ im){
            var data_1orb = new double[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
            for (int ix = 0; ix < data.GetLength(0); ++ix)
            for (int iy = 0; iy < data.GetLength(1); ++iy)
            for (int iz = 0; iz < data.GetLength(2); ++iz)
                data_1orb[ix, iy, iz] = data[ix,iy,iz,im];
            data_.Add(data_1orb);
        }

        return new CubeData(
            commentLines, nAtoms, origin, nVoxels, axes,
            atomicNumbers, nuclearCharges, atomPositions, data_,
            true, nVals, new List<int>(dsetIds)
        );
    }

    static void GetGridParams(double[,] atomPositions,
                                      int[] nVoxels,
                                      double[] origin, double[,] axes)
    {
        double[] centerOfMass = { 0, 0, 0 };
        int nAtom = atomPositions.GetLength(0);

        for (int iaxis = 0; iaxis < 3; ++iaxis){
            for (int iatom = 0; iatom < nAtom; ++iatom){
                centerOfMass[iaxis] += atomPositions[iatom, iaxis];
            }
            centerOfMass[iaxis] /= 3;
        }

        double maxR2 = 0;
        for (int iatom = 0; iatom < nAtom; ++iatom){
            double r2 = 0;
            for (int iaxis = 0; iaxis < 3; ++iaxis){
                double diff = atomPositions[iatom, iaxis] - centerOfMass[iaxis];
                r2 += diff * diff;
            }
            if (maxR2 < r2) { maxR2 = r2; }
        }
        double gridRadius = System.Math.Sqrt(maxR2) + 5;

        for (int iaxis = 0; iaxis < 3; ++iaxis){
            origin[iaxis] = centerOfMass[iaxis] - gridRadius;
            for (int jaxis = 0; jaxis < 3; ++jaxis) { axes[iaxis, jaxis] = 0; }
            axes[iaxis, iaxis] = gridRadius * 2 / (nVoxels[iaxis] - 1);
        }
    }

    static double[,,,] CubeGenCore(
        double[,] atomPositions, int[] shellTypes, int[] nPrimitiveGTO, int[] shellToAtomMap,
        double[] primitiveExponents, double[] contractionCoeffs,
        int[] nVoxels, double[] origin, double[,] axes,
        double[] moCoeffs, int[] iMolecularOrbs)
    {
        Console.WriteLine("BGN CubeGenCore");

        var basisSet = new BasisSet(
            atomPositions, shellTypes, nPrimitiveGTO, shellToAtomMap, 
            primitiveExponents, contractionCoeffs);

        int nAO = basisSet.NAtomicOrb;
        int nMO = iMolecularOrbs.Length;
        double[,,,] data = new double[nVoxels[0],nVoxels[1],nVoxels[2],nMO];
        double[] pos = new double[3];

        pos[0] = origin[0];
        for (int ix = 0; ix < nVoxels[0]; ++ix, pos[0] += axes[0, 0]){
            pos[1] = origin[1];
            for (int iy = 0; iy < nVoxels[1]; ++iy, pos[1] += axes[1, 1]){
                pos[2] = origin[2];
                for (int iz = 0; iz < nVoxels[2]; ++iz, pos[2] += axes[2, 2]){
                    double[] valueAO = basisSet.Eval(pos);
                    for (int im = 0; im < nMO; ++im){
                        int indexMO = iMolecularOrbs[im];
                        double valueMO = 0;
                        for (int iao = 0; iao < nAO; ++iao){
                            valueMO += moCoeffs[indexMO * nAO + iao] * valueAO[iao];
                        }
                        data[ix, iy, iz, im] = valueMO;
                    }
                }
            }
        }

        System.Console.WriteLine("END CubeGenCore");        
        return data;
    }

    public static IEnumerable<float> CubeGenCore(
        CubeData cubeData, int[] shellTypes, int[] nPrimitiveGTO, int[] shellToAtomMap,
        double[] primitiveExponents, double[] contractionCoeffs,
        double[] moCoeffs, int[] iMolecularOrbs)
    {
        Console.WriteLine("BGN CubeGenCore");

        var basisSet = new BasisSet(
            cubeData.AtomPositions, shellTypes, nPrimitiveGTO, shellToAtomMap,
            primitiveExponents, contractionCoeffs);

        int nAO = basisSet.NAtomicOrb;
        int nMO = iMolecularOrbs.Length;
        //double[,,,] data = new double[cubeData.NVoxels[0], cubeData.NVoxels[1], cubeData.NVoxels[2], nMO];
        var data = new List<double[,,]>();
        for (int im = 0; im < nMO; ++im){ 
            data.Add(new double[cubeData.NVoxels[0], cubeData.NVoxels[1], cubeData.NVoxels[2]]); 
        }
        double[] pos = new double[3];

        int progress_total = cubeData.NVoxels[0] * cubeData.NVoxels[1] * cubeData.NVoxels[2] * nAO * nMO;
        int progress = 0;
        int interval = 10000;
        int next_yield = interval;

        pos[0] = cubeData.Origin[0];
        for (int ix = 0; ix < cubeData.NVoxels[0]; ++ix, pos[0] += cubeData.Axes[0, 0]){
            pos[1] = cubeData.Origin[1];
            for (int iy = 0; iy < cubeData.NVoxels[1]; ++iy, pos[1] += cubeData.Axes[1, 1]){
                pos[2] = cubeData.Origin[2];
                for (int iz = 0; iz < cubeData.NVoxels[2]; ++iz, pos[2] += cubeData.Axes[2, 2]){
                    double[] valueAO = basisSet.Eval(pos);
                    for (int im = 0; im < nMO; ++im){
                        int indexMO = iMolecularOrbs[im];
                        double valueMO = 0;
                        for (int iao = 0; iao < nAO; ++iao)
                        {
                            valueMO += moCoeffs[indexMO * nAO + iao] * valueAO[iao];
                        }
                        data[im][ix, iy, iz] = valueMO;
                    }
                }

                progress += cubeData.NVoxels[2] * nMO * nAO;
                if(progress>next_yield){
                    next_yield = progress + interval;
                    yield return (float)progress / (float)progress_total;
                }
            }
        }

        cubeData.Data = data;

        Console.WriteLine("END CubeGenCore");
        yield return 1f;   
    }

    public static IEnumerable<float> CubeGen(CubeData cubeData, FchkData fchkData, int[] iMolecularOrbs, int nVoxel){
        cubeData.CommentLines = new String[]{ "Foo", "Bar" };
        cubeData.NAtoms = fchkData.NAtoms;
        cubeData.AtomicNumbers = fchkData.AtomicNumbers;
        cubeData.NuclearCharges = fchkData.NuclearCharges;
        cubeData.AtomPositions = fchkData.AtomPositions;

        cubeData.NVoxels = new int[]{ nVoxel, nVoxel, nVoxel };
        cubeData.Origin = new double[3];
        cubeData.Axes = new double[3, 3];
        GetGridParams(cubeData.AtomPositions, cubeData.NVoxels, cubeData.Origin, cubeData.Axes);

        // THIS SECTION IS FOR DEBUG !! DELETE HERE LATER-----
        cubeData.NVoxels = new int[] { 60, 60, 60 };
        //origin  = new double[] {-4.970736,   -4.970736,   -4.970736};
        cubeData.Origin = new double[] { -7.157859, -7.157859, -7.157859 };
        cubeData.Axes = new double[3, 3];
        //axes[0,0] = axes[1,1] = axes[2,2] = 0.168500;
        cubeData.Axes[0, 0] = cubeData.Axes[1, 1] = cubeData.Axes[2, 2] = 0.242639;

        // nVoxels = new int[] {120,120,120 };
        // origin = new double[] { -4.970736*2, -4.970736*2, -4.970736*2 };
        // axes = new double[3, 3];
        // axes[0, 0] = axes[1, 1] = axes[2, 2] = 0.168500;
        // ---------------------------------------------------

        yield return 0f;

        foreach(float progress in CubeGenCore(
            cubeData, fchkData.ShellTypes, fchkData.NPrimitiveGTO, fchkData.ShellToAtomMap,
            fchkData.PrimitiveExponents, fchkData.ContractionCoeffs,
            fchkData.AlphaMOCoeffs, iMolecularOrbs))
        {
            yield return progress;
        }

        cubeData.NVal = iMolecularOrbs.Length;
        cubeData.ContainDsetIds = true;
        //cubeData.DsetIds = new int[cubeData.NVal];
        cubeData.DsetIds = new List<int>();
        for (int i = 0; i < cubeData.NVal; ++i){
            //cubeData.DsetIds[i] = iMolecularOrbs[i] + 1;
            cubeData.DsetIds.Add(iMolecularOrbs[i] + 1);
        }

        //return new CubeData(
        //    commentLines, nAtoms, origin, nVoxels, axes,
        //    atomicNumbers, nuclearCharges, atomPositions, data,
        //    true, nVals, dsetIds
        //);

        yield return 1f;
    }



    public static CubeData CubeGen_noMO(FchkData fchkData, int nVoxel)
    {
        string[] commentLines = { "Foo", "Bar" };
        int nAtoms = fchkData.NAtoms;
        int[] atomicNumbers = fchkData.AtomicNumbers;
        double[] nuclearCharges = fchkData.NuclearCharges;
        double[,] atomPositions = fchkData.AtomPositions;

        int[] nVoxels = { nVoxel, nVoxel, nVoxel };
        double[] origin = new double[3];
        double[,] axes = new double[3, 3];
        GetGridParams(atomPositions, nVoxels, origin, axes);

        // THIS SECTION IS FOR DEBUG !! DELETE HERE LATER-----
        nVoxels = new int[] { 60, 60, 60 };
        //origin  = new double[] {-4.970736,   -4.970736,   -4.970736};
        origin = new double[] { -7.157859, -7.157859, -7.157859 };
        axes = new double[3, 3];
        //axes[0,0] = axes[1,1] = axes[2,2] = 0.168500;
        axes[0, 0] = axes[1, 1] = axes[2, 2] = 0.242639;

        // nVoxels = new int[] {120,120,120 };
        // origin = new double[] { -4.970736*2, -4.970736*2, -4.970736*2 };
        // axes = new double[3, 3];
        // axes[0, 0] = axes[1, 1] = axes[2, 2] = 0.168500;
        // ---------------------------------------------------

        int nVals = 0;
        int[] dsetIds = new int[0];

        return new CubeData(
            commentLines, nAtoms, origin, nVoxels, axes,
            atomicNumbers, nuclearCharges, atomPositions, new List<double[,,]>(),
            true, nVals, new List<int>(dsetIds)
        );
    }


    public static void Main(){
        FchkData fchkData = FchkData.Load("h2o.fchk");

        //int[] iMolecularOrbs = { 0, 1, 2, 3, 4, 5, 6 };
        int[] iMolecularOrbs = new int[fchkData.NAtomicOrbs];
        for(int i=0; i<iMolecularOrbs.Length; ++i){ iMolecularOrbs[i]=i; }
        
        CubeData cubeData =  CubeGenerator.CubeGen(fchkData, iMolecularOrbs, 60);
        cubeData.Save("h2o_deb.cube");
    }
}
