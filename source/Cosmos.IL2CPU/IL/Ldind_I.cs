using System;
using XSharp.Assembler;

using XSharp;
using CPUx86 = XSharp.Assembler.x86;
namespace Cosmos.IL2CPU.X86.IL
{
  [Cosmos.IL2CPU.OpCode(ILOpCode.Code.Ldind_I)]
  public class Ldind_I : ILOp
  {
    public Ldind_I(XSharp.Assembler.Assembler aAsmblr)
        : base(aAsmblr)
    {
    }

    public override void Execute(_MethodInfo aMethod, ILOpCode aOpCode)
    {
      Assemble(Assembler, 4, DebugEnabled);
    }

    public static void Assemble(XSharp.Assembler.Assembler aAssembler, int aSize, bool debugEnabled)
    {
      int xAlignedSize = (int)Align((uint)aSize, 4);

      XS.Comment("address at: [esp]");
      DoNullReferenceCheck(aAssembler, debugEnabled, 0);

      XS.Pop(XSRegisters.EAX);
      for (int i = xAlignedSize - 4; i >= 0; i -= 4)
      {
        XS.Push(XSRegisters.EAX, isIndirect: true, displacement: i);
      }
    }
  }
}
