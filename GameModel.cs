using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftMemory;

namespace MinecraftGameModel
{
    public static class GameModel
    {
        private static byte[] coords = new byte[24];
        private static bool isSaved = false;

        public static void Fly(bool isEnabled) { }
        public static void NoClip(bool isEnabled) { }
        public static string SaveCoords()
        {
            bool isAttached = mem.AttachProcess("Minecraft.Windows");
            if (isAttached)
            {
                IntPtr PointerBase = mem.GetModuleAddress("Minecraft.Windows", "Minecraft.Windows") + 0x036A1FB0;
                int[] offsets = { 0x0, 0x3A8, 0x88, 0x0, 0x138, 0x8, 0x498 };
                IntPtr address = mem.GetPointerAddress(PointerBase, offsets, 7);
                coords = mem.ReadBytes(address, 24);
                isSaved = true;
                float x, y, z;
                x = BitConverter.ToSingle(coords, 0);
                y = BitConverter.ToSingle(coords, 4);
                z = BitConverter.ToSingle(coords, 8);
                return "[%" + x.ToString() + "%] , [%" +
                    y.ToString() + "%] , [%" +
                    z.ToString() + "%] coords has been saved";
            }
            else
            {
                return "Game Not Found!";
            }
        }
        public static string LoadCoords()
        {
            bool isAttached = mem.AttachProcess("Minecraft.Windows");
            if (isAttached)
            {
                if (isSaved)
                {
                    IntPtr PointerBase = mem.GetModuleAddress("Minecraft.Windows", "Minecraft.Windows") + 0x036A1FB0;
                    int[] offsets = { 0x0, 0x3A8, 0x88, 0x0, 0x138, 0x8, 0x498 };
                    IntPtr address = mem.GetPointerAddress(PointerBase, offsets, 7);
                    mem.WriteBytes(address, coords, 24);
                    float x, y, z;
                    x = BitConverter.ToSingle(coords, 0);
                    y = BitConverter.ToSingle(coords, 4);
                    z = BitConverter.ToSingle(coords, 8);
                    return "you have been teleported to [%" + x.ToString() + "%] , [%" + y.ToString() + "%] , [%" + z.ToString()+"%]";
                }
                else return "there is no saved coords";
            }
            else return "Game Not Found!";
        }
        public static void GameMode(char c) { }
        public static string Teleport(float[] _coords)
        {
            bool isAttached = mem.AttachProcess("Minecraft.Windows");
            if (isAttached)
            {
                IntPtr PointerBase = mem.GetModuleAddress("Minecraft.Windows", "Minecraft.Windows") + 0x036A1FB0;
                int[] offsets = { 0x0, 0x3A8, 0x88, 0x0, 0x138, 0x8, 0x498 };
                IntPtr address = mem.GetPointerAddress(PointerBase, offsets, 7);
                List<byte> buffer = new List<byte>();
                float x2, y2, z2;
                x2 = _coords[0] + 0.60f;
                y2 = _coords[1] + 1.80f;
                z2 = _coords[2] + 0.60f;
                byte[] tmp = new byte[24];
                int inc = 0;
                buffer.AddRange(BitConverter.GetBytes(_coords[0]));
                buffer.AddRange(BitConverter.GetBytes(_coords[1]));
                buffer.AddRange(BitConverter.GetBytes(_coords[2]));
                buffer.AddRange(BitConverter.GetBytes(x2));
                buffer.AddRange(BitConverter.GetBytes(y2));
                buffer.AddRange(BitConverter.GetBytes(z2));
                foreach (byte b in buffer)
                {
                    tmp[inc] = b;
                    inc++;
                }
                inc = 0;
                mem.WriteBytes(address, tmp, 24);
                return "you have been teleported to [%" + _coords[0].ToString() + "%] , [%" + _coords[1].ToString() + "%] , [%" + _coords[2].ToString() + "%]";
            }
            else return "Game Not Found!";
        }
    }
}
