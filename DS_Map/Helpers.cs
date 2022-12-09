using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NarcAPI;
using Tao.OpenGl;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using Images;
using Ekona.Images;
using Microsoft.WindowsAPICodePack.Dialogs;
using ScintillaNET;
using ScintillaNET.Utils;
using System.Globalization;
using static DSPRE.ROMFiles.Event;
using static ScintillaNET.Style;
using static OpenTK.Graphics.OpenGL.GL;
using NSMBe4.NSBMD;

namespace DSPRE {
  public class Helpers {
    public static bool disableHandlers = false;
    public static RomInfo romInfo;
    public static bool hideBuildings = new bool();

    //Locate File - buttons
    public static void ExplorerSelect(string path) {
      if (File.Exists(path)) {
        Process.Start("explorer.exe", "/select" + "," + "\"" + path + "\"");
      }
    }

    public static string[] GetTrainerNames() {
      List<string> trainerList = new List<string>();

      /* Store all trainer names and classes */
      TextArchive trainerClasses = new TextArchive(RomInfo.trainerClassMessageNumber);
      TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);
      string trainerPropertiesUnpackedDir = RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir;

      int trainerCount = Directory.GetFiles(trainerPropertiesUnpackedDir).Length;

      for (int i = 0; i < trainerCount; i++) {
        int classMessageID = BitConverter.ToUInt16(DSUtils.ReadFromFile(trainerPropertiesUnpackedDir + "\\" + i.ToString("D4"), startOffset: 1, 2), 0);
        string currentTrainerName;

        if (i < trainerNames.messages.Count) {
          currentTrainerName = trainerNames.messages[i];
        }
        else {
          currentTrainerName = TrainerFile.NAME_NOT_FOUND;
        }

        trainerList.Add("[" + i.ToString("D2") + "] " + trainerClasses.messages[classMessageID] + " " + currentTrainerName);
      }

      return trainerList.ToArray();
    }

    public static void MW_LoadModelTextures(NSBMD model, string textureFolder, int fileID) {
      if (fileID < 0) {
        return;
      }

      string texturePath = textureFolder + "\\" + fileID.ToString("D4");
      model.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out model.Textures, out model.Palettes);
      try {
        model.MatchTextures();
      }
      catch {
      }
    }

    public static void RenderMap(ref NSBMDGlRenderer mapRenderer, ref NSBMDGlRenderer buildingsRenderer, ref MapFile mapFile, float ang, float dist, float elev, float perspective, int width, int height, bool mapTexturesON = true, bool buildingTexturesON = true) {
      #region Useless variables that the rendering API still needs

      MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File ani = new MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File();
      MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File tp = new MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File();
      MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File ca = new MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File();
      int[] aniframeS = new int[0];

      #endregion

      /* Invalidate drawing surfaces */
      Program.MainProgram.mapEditor.mapOpenGlControl.Invalidate();
      Program.MainProgram.eventEditor.eventOpenGlControl.Invalidate();

      /* Adjust rendering settings */
      SetupRenderer(ang, dist, elev, perspective, width, height);

      /* Render the map model */
      mapRenderer.Model = mapFile.mapModel.models[0];
      Gl.glScalef(mapFile.mapModel.models[0].modelScale / 64, mapFile.mapModel.models[0].modelScale / 64, mapFile.mapModel.models[0].modelScale / 64);

      /* Determine if map textures must be rendered */
      if (!mapTexturesON) {
        Gl.glDisable(Gl.GL_TEXTURE_2D);
      }
      else {
        Gl.glEnable(Gl.GL_TEXTURE_2D);
      }

      mapRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, mapFile.mapModel); // Render map model

      if (!hideBuildings) {
        if (buildingTexturesON) {
          Gl.glEnable(Gl.GL_TEXTURE_2D);
        }
        else {
          Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        for (int i = 0; i < mapFile.buildings.Count; i++) {
          NSBMD file = mapFile.buildings[i].NSBMDFile;
          if (file is null) {
            Console.WriteLine("Null building can't be rendered");
          }
          else {
            buildingsRenderer.Model = file.models[0];
            ScaleTranslateRotateBuilding(mapFile.buildings[i]);
            buildingsRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, file);
          }
        }
      }
    }

    private static void SetupRenderer(float ang, float dist, float elev, float perspective, int width, int height) {
      //TODO: improve this
      Gl.glEnable(Gl.GL_RESCALE_NORMAL);
      Gl.glEnable(Gl.GL_COLOR_MATERIAL);
      Gl.glEnable(Gl.GL_DEPTH_TEST);
      Gl.glEnable(Gl.GL_NORMALIZE);
      Gl.glDisable(Gl.GL_CULL_FACE);
      Gl.glFrontFace(Gl.GL_CCW);
      Gl.glClearDepth(1);
      Gl.glEnable(Gl.GL_ALPHA_TEST);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
      Gl.glEnable(Gl.GL_BLEND);
      Gl.glAlphaFunc(Gl.GL_GREATER, 0f);
      Gl.glClearColor(51f / 255f, 51f / 255f, 51f / 255f, 1f);
      float aspect;
      Gl.glViewport(0, 0, width, height);
      aspect = Program.MainProgram.mapEditor.mapOpenGlControl.Width / Program.MainProgram.mapEditor.mapOpenGlControl.Height; //(vp[2] - vp[0]) / (vp[3] - vp[1]);
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluPerspective(perspective, aspect, 0.2f, 500.0f); //0.02f, 32.0f);
      Gl.glTranslatef(0, 0, -dist);
      Gl.glRotatef(elev, 1, 0, 0);
      Gl.glRotatef(ang, 0, 1, 0);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadIdentity();
      Gl.glTranslatef(0, 0, -dist);
      Gl.glRotatef(elev, 1, 0, 0);
      Gl.glRotatef(-ang, 0, 1, 0);
      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
      Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
      Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
      Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
      Gl.glLoadIdentity();
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
      Gl.glColor3f(1.0f, 1.0f, 1.0f);
      Gl.glDepthMask(Gl.GL_TRUE);
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT|Gl.GL_DEPTH_BUFFER_BIT);
    }

    private static void ScaleTranslateRotateBuilding(Building building) {
      float fullXcoord = building.xPosition + building.xFraction / 65536f;
      float fullYcoord = building.yPosition + building.yFraction / 65536f;
      float fullZcoord = building.zPosition + building.zFraction / 65536f;

      float scaleFactor = building.NSBMDFile.models[0].modelScale / 1024;
      float translateFactor = 256 / building.NSBMDFile.models[0].modelScale;

      Gl.glScalef(scaleFactor * building.width, scaleFactor * building.height, scaleFactor * building.length);
      Gl.glTranslatef(fullXcoord * translateFactor / building.width, fullYcoord * translateFactor / building.height, fullZcoord * translateFactor / building.length);
      Gl.glRotatef(Building.U16ToDeg(building.xRotation), 1, 0, 0);
      Gl.glRotatef(Building.U16ToDeg(building.yRotation), 0, 1, 0);
      Gl.glRotatef(Building.U16ToDeg(building.zRotation), 0, 0, 1);
    }

    public static Bitmap GrabMapScreenshot(int width, int height) {
      Bitmap bmp = new Bitmap(width, height);
      System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      Gl.glReadPixels(0, 0, width, height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, data.Scan0);
      bmp.UnlockBits(data);
      bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
      return bmp;
    }
  }
}
