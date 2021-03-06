﻿using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{

    public void ImportModuleLoader()
    {
        const MethodAttributes attributes = MethodAttributes.Static
                                            | MethodAttributes.SpecialName
                                            | MethodAttributes.RTSpecialName;
        var moduleClass = ModuleDefinition.Types.FirstOrDefault(x => x.Name == "<Module>");
        if (moduleClass == null)
        {
            throw new WeavingException("Found no module class!");
        }
        var cctor = moduleClass.Methods.FirstOrDefault(x => x.Name == ".cctor");
        if (cctor == null)
        {
            cctor = new MethodDefinition(".cctor", attributes, VoidTypeReference);
            cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            moduleClass.Methods.Add(cctor);
        }
        cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, AttachMethod));
    }
}