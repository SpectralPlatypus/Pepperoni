using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Utils;


namespace MonoMod
{
    /// <summary>
    /// Patch PlayerMachine JumpSuperUpdate instead of reimplementing it in Pepperoni
    /// </summary>
    [MonoModCustomMethodAttribute("PatchJumpSuperUpdate")]
    class PatchJumpSuperUpdateAttribute : Attribute { }

    static class MonoModRules
    {
        public static void PatchJumpSuperUpdate(MethodDefinition method, CustomAttribute attrib)
        {
            if (!method.HasBody)
                return;

            MethodDefinition m_IsCoyoteFrameEnabled = method.DeclaringType.FindMethod("System.Boolean _IsCoyoteFrameEnabled(System.Boolean,PlayerMachine)");
            if (m_IsCoyoteFrameEnabled == null)
               return;

            Mono.Collections.Generic.Collection<Instruction> instrs = method.Body.Instructions;
            ILProcessor il = method.Body.GetILProcessor();

            if (instrs[0].OpCode == OpCodes.Ldsfld &&
                instrs[1].OpCode == OpCodes.Callvirt)
            {
                //Looking for something like this:
                // ldarg.0
                // ldfld float32 PlayerMachine::FallTime
                // ldc.r4 2
                // bge.un.s IL_00ff

                Instruction jumpTo = null;
                int j = -1;
                for (int k = 2; k < instrs.Count - 2; ++k)
                {
                    if(instrs[k].OpCode == OpCodes.Brfalse && instrs[k-1].OpCode == OpCodes.Ldfld &&
                       (instrs[k-1].Operand as FieldReference).FullName == "System.Boolean PlayerMachine::FallJumpWindowJumpCheck")
                    {
                        instrs[k] = il.Create(OpCodes.Nop);
                    }
                    
                    if(instrs[k].OpCode == OpCodes.Bge_Un && instrs[k - 1].OpCode == OpCodes.Ldfld &&
                       (instrs[k - 1].Operand as FieldReference).FullName == "System.Single PlayerMachine::FallJumpWindow")
                    {
                        instrs[k] = il.Create(OpCodes.Clt_Un);
                        ++k;
                        instrs.Insert(k, il.Create(OpCodes.And));
                        j = ++k;
                    }

                        if (instrs[k].OpCode == OpCodes.Ldarg_0 && instrs[k + 1].OpCode == OpCodes.Ldfld && instrs[k + 2].OpCode == OpCodes.Ldc_R4
                        && (instrs[k+1].Operand as FieldReference).FullName == "System.Single PlayerMachine::FallTime")
                    {
                        jumpTo = instrs[k];
                        break;
                    }
                }

                if (jumpTo == null || j <= 0) return;

                //Push this
                instrs.Insert(j, il.Create(OpCodes.Ldarg_0));
                ++j;
                // Process.
                instrs.Insert(j, il.Create(OpCodes.Call, m_IsCoyoteFrameEnabled));
                ++j;
                instrs.Insert(j, il.Create(OpCodes.Brfalse, jumpTo));         
            }
            
        }
    }
}