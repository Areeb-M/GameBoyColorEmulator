using System;
using System.Collections;
using System.Collections.Generic;

namespace Emulator
{
	class OpcodeTable
	{	
		public delegate void OpcodeFunction(CPU cpu, Memory mem);
		public static Dictionary<byte, OpcodeFunction> OPCODE_TABLE;
		
		public static void GenerateOpcodeTable()
		{
			OpcodeFunction nop = NOP;
			OpcodeFunction jump = JUMP;
			OpcodeFunction restart38 = RESTART38;
			OpcodeFunction compare = COMPARE;
			OpcodeFunction jumpForward = JUMP_FORWARD;
			OpcodeFunction jumpForwardIf = JUMP_FORWARD_IF;
			OpcodeFunction xor = XOR;
			OpcodeFunction loadNNintoN = LOAD_NN_INTO_N;
			OpcodeFunction disableInterrupts = DISABLE_INTERRUPTS;
			OpcodeFunction loadAIntoN = LOAD_A_INTO_N;
			OpcodeFunction addRegToA = ADD_REG_TO_A;
			OpcodeFunction callNN = CALL_NN;
			OPCODE_TABLE = new Dictionary<byte, OpcodeFunction>()
			{
				{0x00, nop},
				{0x01, loadNNintoN},
				{0x18, jumpForward},
				{0x28, jumpForwardIf},
				{0x3E, nop}, // This opcode doesn't exist on the online table
				{0x80, addRegToA},
				{0xA8, xor},
				{0xA9, xor},
				{0xAA, xor},
				{0xAB, xor},
				{0xAC, xor},
				{0xAD, xor},
				{0xAE, xor},
				{0xAF, xor},
				{0xC3, jump},
				{0xCD, callNN},
				{0xE0, loadAIntoN},
				{0xEE, xor},
				{0xF3, disableInterrupts},
				{0xFE, compare},
				{0xFF, restart38},	
			};
		}
			
	/*	Template Function
		public static void name(CPU cpu, Memory mem)
		{
			
		}
	*/
		public static void NOP(CPU cpu, Memory mem)
		{
			// NOP: does nothing
			cpu.PC += 1; // opcode length
			Debug.Log(": NOP");
		}
		
		public static void JUMP(CPU cpu, Memory mem)
		{
			// JUMP: Jumps to a location in memory
			cpu.PC = mem[cpu.PC+1] + (mem[cpu.PC+2] << 8);
			Debug.Log(": Jump to {0:X4}", cpu.PC);
		}
		
		public static void RESTART38(CPU cpu, Memory mem)
		{
			// Restart38: Restarts Gameboy from memory location 0x38
			mem[--cpu.SP] = (byte)(cpu.PC & 0x00FF);
			mem[--cpu.SP] = (byte)((cpu.PC & 0xFF00) >> 8);
			cpu.PC = 0x0038 + 1;
			Debug.Log(": restart from 0x0038");
		}
		
		public static void COMPARE(CPU cpu, Memory mem)
		{
			int a = cpu.A;
			int b;
			switch(mem[cpu.PC]){
				case 0xFE:
					b = mem[++cpu.PC];
					break;
				default:
					Debug.Log("\n[Error]Unimplemented compare opcode detected!");
					return;
			}
			int result = a - b;
			int f = 0x0000;
			f += ((result == 0) ? 1 : 0); f <<= 1;         // Z Flag
			f += 1; f <<= 2;                   // N Flag
			f += (((a&0xF) + ((-b)&0xF))&0x10)>>3; // H Flag
			f += ((result < 0) ? 1 : 0); f <<= 4;
			cpu.F = (byte)f;
			cpu.PC += 1;
			Debug.Log(": Compare {1} - {2} reg[F] = {0}", Convert.ToString(cpu.F, 2).PadLeft(8, '0'), a, b);
		}
		
		public static void JUMP_FORWARD_IF(CPU cpu, Memory mem)
		{
			switch(mem[cpu.PC]){
				case 0x28:
					if ((cpu.F & 0x80) == 0x80){
						JUMP_FORWARD(cpu, mem);
					} else {
						cpu.PC += 2;
						Debug.Log(": Jump forward failed since Z flag was not set");
					}
					break;
				default:
					Debug.Log("\n[Error]Unimplemented jumpForwardIf opcode detected!");
					break;
			}
		}
		
		public static void JUMP_FORWARD(CPU cpu, Memory mem)
		{
			cpu.PC += mem[cpu.PC+1];
			Debug.Log(": Jump forward to {0:X4}", cpu.PC);
		}
		
		public static void XOR(CPU cpu, Memory mem)
		{
			int a = cpu.A;
			int b;
			switch (mem[cpu.PC]){
				case 0xAF:
					b = cpu.A;
					cpu.PC += 1;
					break;
				default:
					Debug.Log("\n[Error]Unimplemented XOR opcode detected!");
					return;
			}
			cpu.A = (byte)(a ^ b);
			cpu.F = (cpu.A == 0) ? (byte)0x80 : (byte)0;
		Debug.Log(": XOR reg[A]={0} with b={1} to get {2}. Store in reg[A]. reg[F] = ", a, b, cpu.A);
		Debug.PrintBinary(cpu.F);
		}
		
		public static void LOAD_NN_INTO_N(CPU cpu, Memory mem)
		{
			int nn = mem[cpu.PC+2] << 8 + mem[cpu.PC+1];
			switch(mem[cpu.PC])
			{
				case 0x01:
					cpu.BC = nn;
					Debug.Log(": Store {0:X4} into regBC", nn);
					break;
				default:
					Debug.Log("\n[Error]Unimplemented LOAD_NN_INTO_N opcode detected!");
					break;					
			}
			cpu.PC += 3;
		}
		
		public static void DISABLE_INTERRUPTS(CPU cpu, Memory mem)
		{			
			if (cpu.Interrupts)
				cpu.ToggleInterrupts = true;
			cpu.PC += 1;
			Debug.Log(": Disable interrupts opcode");
		}
				
		public static void LOAD_A_INTO_N(CPU cpu, Memory mem)
		{
			int address = 0xFF00 + mem[++cpu.PC];
			mem[address] = cpu.A;
			
			cpu.PC += 1;
			Debug.Log(": Loaded regA({1:X2}) into {0:X4}", address, cpu.A);
		}
		
		public static void ADD_REG_TO_A(CPU cpu, Memory mem)
		{
			byte a = cpu.A;
			byte b;
			Debug.Log(": Add regA({0}) and reg", a);
			switch(mem[cpu.PC])
			{
				case 0x80:
					b = cpu.B;
					Debug.Log("B({0})", b);
					break;
				default:
					Debug.Log("\n[Error]Unimplemented addREGtoA opcode detected!");
					return;				
			}
			byte result = (byte)(a + b);
			cpu.A = result;
			int f = 0x00;
			f += ((result == 0) ? 1 : 0); f <<= 1;         // Z Flag
			f <<= 2;                   // Reset N Flag
			f += (((a&0xF) + (b&0xF)) & 0x10) >> 4; f <<= 1; // Half Carry flag
			f += (((a&0xF0) + (b&0xF0)) & 0x100) >> 8; // Full Carry flag
			f <<= 4;
			cpu.F = (byte)f;
			
			cpu.PC += 1;
			
			Debug.Log(" to get {0} and store it in regA - regF is ", result);
			Debug.PrintBinary(cpu.F);
		}
		
		public static void CALL_NN(CPU cpu, Memory mem)
		{
			cpu.SP -= 1;
			mem[cpu.SP] = (byte)((cpu.PC & 0xFF00) >> 4);
			cpu.SP -= 1;
			mem[cpu.SP] = (byte)(cpu.PC & 0xFF);
			
			Debug.Log(": Push {0:X4} onto the stack ", cpu.PC);
			
			JUMP(cpu, mem);			
		}
	/*
		delegate void OpcodeFunction();
		Dictionary<int, OpcodeFunction> OPCODE_TABLE;	
			OpcodeFunction nop = NOP;
			OpcodeFunction jump = JUMP;
			OpcodeFunction restart38 = RESTART38;
			OpcodeFunction compare = COMPARE;
			OpcodeFunction jumpForward = JUMP_FORWARD;
			OpcodeFunction jumpForwardIf = JUMP_FORWARD_IF;
			OpcodeFunction xor = XOR;
			OpcodeFunction loadNIntoA = LOAD_N_INTO_A;
			OpcodeFunction loadAIntoN = LOAD_A_INTO_N;
			OpcodeFunction disableInterrupts = DISABLE_INTERRUPTS;
			opcodeTable = new Dictionary<int, OpcodeFunction>(){
					{0x00, nop},
					{0x0A, loadNIntoA},
					{0x18, jumpForward},
					{0x1A, loadNIntoA},
					{0x28, jumpForwardIf},
					{0x3E, loadNIntoA},
					{0x78, loadNIntoA},
					{0x79, loadNIntoA},
					{0x7A, loadNIntoA},
					{0x7B, loadNIntoA},
					{0x7C, loadNIntoA},
					{0x7D, loadNIntoA},
					{0x7E, loadNIntoA},
					{0x7F, loadNIntoA},
					{0xA8, xor},
					{0xA9, xor},
					{0xAA, xor},
					{0xAB, xor},
					{0xAC, xor},
					{0xAD, xor},
					{0xAE, xor},
					{0xAF, xor},
					{0xC3, jump},
					{0xE0, loadAIntoN},
					{0xEE, xor},
					{0xF3, disableInterrupts},
					{0xFA, loadNIntoA},
					{0xFE, compare},
					{0xFF, restart38},
			};

		public static void NOP(CPU cpu)
		{
			
		}


		
		private void NOP(){
			// NOP: Doesn't do anything
			PC += 1; // OPCODE length
		}
	
		private void JUMP(){ 
			// JUMP: Jumps to a location in memory
			PC = memory[PC+1] + (memory[PC+2] << 8);
			Console.Write(": Jump to {0:X4}", PC);
		}
		
		private void RESTART38(){
			// Restart38: Restarts Gameboy from memory location 0x38
			memory.write(--SP, (byte)(PC & 0x00FF));
			memory.write(--SP, (byte)((PC & 0xFF00) >> 8));
			PC = 0x0038 + 1;
		}
		
		private void COMPARE(){
			int a = reg[A];
			int b;
			switch(memory[PC]){
				case 0xFE:
					b = memory[++PC];
					break;
				default:
					Console.WriteLine("\n[Error]Unimplemented compare opcode detected!");
					return;
			}
			int result = a - b;
			int f = 0x0000;
			f += ((result == 0) ? 1 : 0); f <<= 1*4;         // Z Flag
			f += 1; f <<= 2*4;                   // N Flag
			f += ((a&0xF) + ((-b)&0xF))&0x10; // H Flag
			f += ((result < 0) ? 1 : 0); f <<= 4*4;
			reg[F] = (byte)f;
			Console.Write(": Compare results reg[F] = {0}", Convert.ToString(reg[F], 2).PadLeft(8, '0'));
			PC += 1;
		}	
		
		private void JUMP_FORWARD_IF(){
			switch(memory[PC]){
				case 0x28:
					if ((reg[F] & 0x80) == 0x80){
						JUMP_FORWARD();
					} else {
						PC += 2;
						Console.Write(": Jump forward failed since Z flag was not set");
					}
					break;
				default:
					Console.WriteLine("\n[Error]Unimplemented jumpForwardIf opcode detected!");
					break;
			}			
		}
		
		private void JUMP_FORWARD(){
			PC += memory[PC+1];
			Console.Write(": Jump forward to {0:X4}", PC);
		}
		
		private void XOR(){
			int a = reg[A];
			int b;
			switch (memory[PC]){
				case 0xAF:
					b = reg[A];
					PC += 1;
					break;
				default:
					Console.WriteLine("\n[Error]Unimplemented XOR opcode detected!");
					return;
			}
			reg[A] = (byte)(a ^ b);
			reg[F] = (reg[A] == 0) ? (byte)0x80 : (byte)0;
			Debug.XOR(a, b, reg[A]);
		}
		
		private void LOAD_N_INTO_A(){
			int opcode = memory[PC];
			byte val;
			switch (opcode){
				case 0x3E:
					val = memory[++PC];
					break;
				default:
					Console.WriteLine("\n[Error]Unimplemented load(N)IntoA opcode detected!");
					return;
			}
			reg[A] = val;
			Debug.LOAD("reg[A]", val);
			
			PC += 1;
		}
		
		private void LOAD_A_INTO_N(){
			int address = 0xFF00 + memory[++PC];
			memory.write(address, reg[A]);
			Debug.LOADH(address, reg[A]);
			
			PC += 1;
		}
		
		private void DISABLE_INTERRUPTS(){
			if (interrupts)
				toggleInterrupts = true;
			PC += 1;
		}
		*/
	}

}