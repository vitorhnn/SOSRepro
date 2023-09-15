using System;
using System.Reflection;
using System.Text.Json;

namespace SOSRepro
{
    internal static class Class1
    {
        internal static void Thing()
        {
            var j = JsonSerializer.Serialize("banana");
            Console.WriteLine(j);

            var currentDomain = AppDomain.CurrentDomain;

            foreach (var asm in currentDomain.GetAssemblies())
            {
                Console.WriteLine(asm.ToString());
                Console.WriteLine(asm.GetName().ToString());
            }
        }
    }
    internal static class Program
    {
        private static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Class1.Thing();
        }

        private static Assembly LoadFromResources(AssemblyName asmname)
        {
            var self = Assembly.GetExecutingAssembly();
            var resname = "SOSRepro." + asmname.Name + ".dll";
            Console.WriteLine("attempting to load " + asmname);

            using (var stream = self.GetManifestResourceStream(resname))
            {
                if (stream == null)
                {
                    Console.WriteLine("no embedded asm " + resname);
                    return null;
                }
                var buf = new byte[stream.Length];
                if (stream.Read(buf, 0, buf.Length) != buf.Length) 
                    return null;
                
                var asm = Assembly.Load(buf);
                Console.WriteLine("loaded asm " + asm.ToString());
                return asm;
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmname = new AssemblyName(args.Name);
            return LoadFromResources(asmname);
        }
    }
}
