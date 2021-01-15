// OpenGL Renderer for NSBMD models.
// Code adapted from kiwi.ds' NSBMD Model Viewer.

using System;
using System.Collections.Generic;
using Tao.OpenGl;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using OpenTK;
using HelixToolkit;
using MKDS_Course_Editor.Export3DTools;

namespace LibNDSFormats.NSBMD
{
	/// <summary>
	/// OpenGL renderer for NSBMD models.
	/// </summary>
	public class NSBMDGlRenderer
	{
		#region Fields (11)
		private bool writevertex = true;
		public List<float[]> normals = new List<float[]>();
		public List<float[]> vertex = new List<float[]>();
		List<HelixToolkit.MeshBuilder> md = new List<HelixToolkit.MeshBuilder>();
        public static DependencyProperty matName = DependencyProperty.Register("MatName", typeof(string), typeof(DiffuseMaterial));
        public static DependencyProperty polyName = DependencyProperty.Register("PolyName", typeof(string), typeof(DiffuseMaterial));
		List<ImageBrush> matt = new List<ImageBrush>();
		List<ImageBrush> mattt = new List<ImageBrush>();
		public List<int> vertex_normal = new List<int>();
		private static MTX44 CurrentMatrix;
		private static bool g_mat = true;
		private static string file;
		public static bool gOptColoring = true;
		private static bool gOptTexture = true;

		private static bool gOptWireFrame = false;
		private static MTX44[] MatrixStack = new MTX44[31];
		private const float SCALE_IV = 4096.0f;
		private static int stackID;

		private int[] Tx;
		private int[] Ty;
		private int[] Tz;
		//private int[] T2;
		private int[] R;
		//private int[] R2;
		private int[] Sx;
		private int[] Sy;
		private int[] Sz;
		private int[] S2x;
		private int[] S2y;
		private int[] S2z;
		//private int[] S2;
		private int[] frame;
		private int[] nr;
		private int[] frame_;

		//private static int gCurrentPoly;
		private static int gCurrentVertex;
		private static bool gOptVertexMode;
		private int matstart;
		private static readonly String[] TEXTURE_FORMATS = new String[] { "", "A3I5", "4-Color", "16-Color", "256-Color", "4x4-Texel", "A5I3", "Direct Texture" };

		#endregion Fields

		#region Constructors (1)

		public NSBMDGlRenderer()
		{
			this.matstart = 0;
			// Init matrix stack.
			for (int i = 0; i < MatrixStack.Length; ++i)
				MatrixStack[i] = new MTX44();
		}
		/// <summary>
		/// Ctor.
		/// </summary>
		public NSBMDGlRenderer(int matstart)
		{
			this.matstart = matstart;
			// Init matrix stack.
			for (int i = 0; i < MatrixStack.Length; ++i)
				MatrixStack[i] = new MTX44();
		}

		#endregion Constructors

		#region Methods (8)

		// Public Methods (1) 

		/// <summary>
		/// Model to render.
		/// </summary>
		private NSBMDModel _model;

		/// <summary>
		/// Model to render.
		/// </summary>
		public NSBMDModel Model
		{
			get { return _model; }
			set
			{
				_model = value;
				try
				{
                    MakeTexture(_model);
				}
				catch
				{

				}
			}
		}
		public enum RenderMode
		{
			Opaque = 1,
			Translucent,
			Picking
		}
		public static float[] rotate(float[] a, float x, float y, float z)
		{
			float[] b = loadIdentity();
			float cx = (float)Math.Cos(x);
			float sx = (float)Math.Sin(x);
			float cy = (float)Math.Cos(y);
			float sy = (float)Math.Sin(y);
			float cz = (float)Math.Cos(z);
			float sz = (float)Math.Sin(z);
			b[0] = cy * cz;
			b[1] = cy * sz;
			b[2] = -sy;
			b[4] = cz * sx * sy - sz * cx;
			b[5] = sx * sy + cx * cz;
			b[6] = sx * cy;
			b[8] = cx * cz * sy + sx * sz;
			b[9] = cx * sy * sz - sx * cz;
			b[10] = cx * cy;
			return multMatrix(a, b);
		}
		int lastselectedanim = -2;
		public void RenderModel(string file2, MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File ani, int[] aniframeS, int[] aniframeT, int[] aniframeScaleS, int[] aniframeScaleT, int[] aniframeR, MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File ca, bool anim, int selectedani, float X, float Y, float dist, float elev, float ang, MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File p, NSBMD nsb)
		{
			RenderModel(file2, ani, aniframeS, aniframeT, aniframeScaleS, aniframeScaleT, aniframeR, ca, anim, selectedani, X, Y, dist, elev, ang, true, p, nsb);
		}
		public void RenderModel(string file2, MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File ani, int[] aniframeS, int[] aniframeT, int[] aniframeScaleS, int[] aniframeScaleT, int[] aniframeR, MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File ca, bool anim, int selectedani, float X, float Y, float dist, float elev, float ang, bool licht, MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File p, NSBMD nsb)
		{
			if (lastselectedanim != selectedani && selectedani != -1)
			{
				Tx = new int[ca.JAC[selectedani].ObjInfo.Length];
				Ty = new int[ca.JAC[selectedani].ObjInfo.Length];
				Tz = new int[ca.JAC[selectedani].ObjInfo.Length];
				R = new int[ca.JAC[selectedani].ObjInfo.Length];
				Sx = new int[ca.JAC[selectedani].ObjInfo.Length];
				Sy = new int[ca.JAC[selectedani].ObjInfo.Length];
				Sz = new int[ca.JAC[selectedani].ObjInfo.Length];
				S2x = new int[ca.JAC[selectedani].ObjInfo.Length];
				S2y = new int[ca.JAC[selectedani].ObjInfo.Length];
				S2z = new int[ca.JAC[selectedani].ObjInfo.Length];
				lastselectedanim = selectedani;
			}
			if (frame == null && p.Header.file_size != 0)
			{
				frame = new int[p.MPT.names.Length];
				frame_ = new int[p.MPT.names.Length];
				nr = new int[p.MPT.names.Length];
			}
			RenderModel(file2, ani, aniframeS, aniframeT, aniframeScaleS, aniframeScaleT, aniframeR, ca, RenderMode.Opaque, anim, anim, selectedani, X, Y, dist, elev, ang, licht, p, nsb); //anim, anim);
			RenderModel(file2, ani, aniframeS, aniframeT, aniframeScaleS, aniframeScaleT, aniframeR, ca, RenderMode.Translucent, false, anim, selectedani, X, Y, dist, elev, ang, licht, p, nsb);
		}
		//MTX44[] mt;
		/// <summary>
		/// Render model to OpenGL surface.
		/// </summary>
		public void RenderModel(string file2, MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File ani, int[] aniframeS, int[] aniframeT, int[] aniframeScaleS, int[] aniframeScaleT, int[] aniframeR, MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File ca, RenderMode r, bool anim, bool anim2, int selectedanim, float X, float Y, float dist, float elev, float ang, bool licht, MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File p, NSBMD nsb)
		{
			MTX44 tmp = new MTX44();
			file = file2;
			for (var j = 0; j < Model.Polygons.Count - 1; j++)
			{
				var poly = Model.Polygons[j];
				int matid = poly.MatId;
				var mat = Model.Materials[matid];
			}
			int light = Gl.glIsEnabled(Gl.GL_LIGHTING);
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glLineWidth(2.0F);

			if (light == 1)
			{
				Gl.glEnable(Gl.GL_LIGHTING);
			}
			Gl.glLineWidth(1.0F);

			////////////////////////////////////////////////////////////
			// prepare the matrix stack
            for (var i = 0; i < Model.Objects.Count; i++)
            {
                var obj = Model.Objects[i];
                var m_trans = obj.TransVect;


                if (obj.RestoreID != -1)
                    Gl.glLoadMatrixf(MatrixStack[obj.RestoreID].Floats);
                if (obj.StackID != -1)
                {
                    if (obj.Trans)
                        Gl.glTranslatef(m_trans[0], m_trans[1], m_trans[2]);


                    Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, MatrixStack[obj.StackID].Floats);
                    stackID = obj.StackID; // save the last stackID
                }
            }
            Gl.glLoadIdentity();
			////////////////////////////////////////////////////////////
			// display one polygon of the current model at a time
			//Todo

			////////////////////////////////////////////////////////////
			// display all polygons of the current model
			for (var i = 0; i < Model.Polygons.Count - 1; i++)
			{
				var poly = Model.Polygons[i];

				if (gOptTexture && !gOptWireFrame && g_mat)
				{
					int matid = poly.MatId;
					if (matid == -1)
					{
						Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
						if (writevertex)
						{
							mattt.Add(new ImageBrush());
						}
					}
					else
					{
						if (writevertex)
						{
							mattt.Add(matt[matid]);
						}
						NSBMDMaterial mat = Model.Materials[matid];
						if ((mat.format == 1 || mat.format == 6) && r == RenderMode.Translucent) continue;
						if ((mat.format == 0 || mat.format == 2 || mat.format == 3 || mat.format == 4 || mat.format == 5 || mat.format == 7) && r != RenderMode.Opaque) continue;
						Gl.glBindTexture(Gl.GL_TEXTURE_2D, matid + 1 + matstart);

						// Convert pixel coords to normalised STs
						Gl.glMatrixMode(Gl.GL_TEXTURE);
						Gl.glLoadIdentity();
						if (p.Header.file_size != 0 && new List<string>(p.MPT.names).Contains(mat.MaterialName))
						{
							NSBMDMaterial mmm = mat;
							int texid = 0;
							for (int l = 0; l < nsb.Textures.Count; l++)
							{
								if (nsb.Textures[l].texname == p.AnimData[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)].KeyFrames[frame_[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)]].texName) { texid = l; break; }
							}
							mmm.spdata = nsb.Textures[texid].spdata;
							mmm.texdata = nsb.Textures[texid].texdata;
							mmm.texname = nsb.Textures[texid].texname;
							mmm.texoffset = nsb.Textures[texid].texoffset;
							mmm.texsize = nsb.Textures[texid].texsize;
							mmm.width = nsb.Textures[texid].width;
							mmm.height = nsb.Textures[texid].height;
							mmm.format = nsb.Textures[texid].format;
							mmm.color0 = nsb.Textures[texid].color0;

							int palid = 0;
							for (int l = 0; l < nsb.Textures.Count; l++)
							{
								if (nsb.Palettes[l].palname == p.AnimData[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)].KeyFrames[frame_[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)]].palName) { palid = l; break; }
							}
							mmm.paldata = nsb.Palettes[palid].paldata;
							mmm.palname = nsb.Palettes[palid].palname;
							mmm.paloffset = nsb.Palettes[palid].paloffset;
							mmm.palsize = nsb.Palettes[palid].palsize;
							MakeTexture(matid, mmm);
							if (anim2)
							{
								if (nr[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)] == Math.Round((float)(p.MPT.infoBlock.Data[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)].Unknown1) / 512f))
								{
									nr[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)] = 0;
									if (frame[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)] == p.MPT.NoFrames - 1)
									{
										frame[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)] = 0;
										frame_[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)] = 0;
									}
									else
									{
										frame[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)]++;
										if (p.AnimData[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)].KeyFrames.Length != frame_[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)] + 1)
										{
											if (frame[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)] == p.AnimData[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)].KeyFrames[frame_[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)] + 1].Start)
											{
												frame_[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)]++;
											}
										}
									}
								}
								else
								{
									nr[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)]++;//= (float)p.MPT.infoBlock.Data[new List<string>(p.MPT.names).IndexOf(mat.MaterialName)].Unknown1 / 4096f;
								}
							}
						}
						try
						{
							if (ani.Header.file_size != 0 && new List<string>(ani.MAT.names).Contains(mat.MaterialName))
							{
								int index = new List<string>(ani.MAT.names).IndexOf(mat.MaterialName);
								Gl.glScaled((double)ani.SRTData[index].scaleS[aniframeScaleS[index]], (double)ani.SRTData[index].scaleT[aniframeScaleT[index]], 1);
								Gl.glRotated((double)ani.SRTData[index].rotate[aniframeR[index]], 1, 0, 0);
								Gl.glTranslated((double)ani.SRTData[index].translateS[aniframeS[index]], (double)ani.SRTData[index].translateT[aniframeT[index]], 0);
								if (anim2)
								{
									if (aniframeS[index] == ani.SRTData[index].translateS.Length - 1)
									{
										aniframeS[index] = 0;
									}
									else
									{
										aniframeS[index]++;
									}
									if (aniframeT[index] == ani.SRTData[index].translateT.Length - 1)
									{
										aniframeT[index] = 0;
									}
									else
									{
										aniframeT[index]++;
									}
									if (aniframeR[index] == (ani.SRTData[index].rotate.Length - 2) / 2)
									{
										aniframeR[index] = 0;
									}
									else
									{
										aniframeR[index]++;
									}
									if (aniframeScaleS[index] == ani.SRTData[index].scaleS.Length - 1)
									{
										aniframeScaleS[index] = 0;
									}
									else
									{
										aniframeScaleS[index]++;
									}
									if (aniframeScaleT[index] == ani.SRTData[index].scaleT.Length - 1)
									{
										aniframeScaleT[index] = 0;
									}
									else
									{
										aniframeScaleT[index]++;
									}
								}
								goto noscale;
							}
							else
							{
								goto scale;
							}
						}
						catch
						{

						}
					noscale:
						if (!mat.isEnvironmentMap)
						{
							Gl.glScalef(1.0f / ((float)mat.width), 1.0f / ((float)mat.height), 1.0f);
						}
						goto end;
					scale:
						if (!mat.isEnvironmentMap)
						{
							if (mat.mtx == null)
							{
								Gl.glScalef((float)mat.scaleS / (mat.width), (mat.scaleT / mat.height), 1.0f);
								Gl.glRotatef(mat.rot, 0, 1, 0);
								Gl.glTranslatef(mat.transS, mat.transT, 0);
							}
							else
							{
								Gl.glScalef(1.0f / ((float)mat.width), 1.0f / ((float)mat.height), 1.0f);
								Gl.glMultMatrixf(mat.mtx);
							}
						}
					end:

						Gl.glEnable(Gl.GL_ALPHA_TEST);
						Gl.glAlphaFunc(Gl.GL_GREATER, 0f);
						Gl.glColor4f(0xff, 0xff, 0xff, 0xff);

						if (licht && (((mat.PolyAttrib >> 0) & 0x1) == 0 && ((mat.PolyAttrib >> 1) & 0x1) == 0 && ((mat.PolyAttrib >> 2) & 0x1) == 0 && ((mat.PolyAttrib >> 3) & 0x1) == 0) == false)
						{
							//Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
							Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, new float[] { (float)mat.DiffuseColor.R / 255f, (float)mat.DiffuseColor.G / 255f, (float)mat.DiffuseColor.B / 255f, (float)mat.DiffuseColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, new float[] { (float)mat.AmbientColor.R / 255f, (float)mat.AmbientColor.G / 255f, (float)mat.AmbientColor.B / 255f, (float)mat.AmbientColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, new float[] { (float)mat.SpecularColor.R / 255f, (float)mat.SpecularColor.G / 255f, (float)mat.SpecularColor.B / 255f, (float)mat.SpecularColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_EMISSION, new float[] { (float)mat.EmissionColor.R / 255f, (float)mat.EmissionColor.G / 255f, (float)mat.EmissionColor.B / 255f, (float)mat.EmissionColor.A / 255f });
							//Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
							Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, new float[] { (float)mat.DiffuseColor.R / 255f, (float)mat.DiffuseColor.G / 255f, (float)mat.DiffuseColor.B / 255f, (float)mat.DiffuseColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, new float[] { (float)mat.AmbientColor.R / 255f, (float)mat.AmbientColor.G / 255f, (float)mat.AmbientColor.B / 255f, (float)mat.AmbientColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_SPECULAR, new float[] { (float)mat.SpecularColor.R / 255f, (float)mat.SpecularColor.G / 255f, (float)mat.SpecularColor.B / 255f, (float)mat.SpecularColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_EMISSION, new float[] { (float)mat.EmissionColor.R / 255f, (float)mat.EmissionColor.G / 255f, (float)mat.EmissionColor.B / 255f, (float)mat.EmissionColor.A / 255f });
							//Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, new float[] { 1.0f, 1.0f, 1.0f, 0 });
							Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, new float[] { (float)mat.DiffuseColor.R / 255f, (float)mat.DiffuseColor.G / 255f, (float)mat.DiffuseColor.B / 255f, (float)mat.DiffuseColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_AMBIENT, new float[] { (float)mat.AmbientColor.R / 255f, (float)mat.AmbientColor.G / 255f, (float)mat.AmbientColor.B / 255f, (float)mat.AmbientColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_SPECULAR, new float[] { (float)mat.SpecularColor.R / 255f, (float)mat.SpecularColor.G / 255f, (float)mat.SpecularColor.B / 255f, (float)mat.SpecularColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_EMISSION, new float[] { (float)mat.EmissionColor.R / 255f, (float)mat.EmissionColor.G / 255f, (float)mat.EmissionColor.B / 255f, (float)mat.EmissionColor.A / 255f });
							//Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, new float[] { 1.0f, 1.0f, 1.0f, 0 });
							Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, new float[] { (float)mat.DiffuseColor.R / 255f, (float)mat.DiffuseColor.G / 255f, (float)mat.DiffuseColor.B / 255f, (float)mat.DiffuseColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_AMBIENT, new float[] { (float)mat.AmbientColor.R / 255f, (float)mat.AmbientColor.G / 255f, (float)mat.AmbientColor.B / 255f, (float)mat.AmbientColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_SPECULAR, new float[] { (float)mat.SpecularColor.R / 255f, (float)mat.SpecularColor.G / 255f, (float)mat.SpecularColor.B / 255f, (float)mat.SpecularColor.A / 255f });
							Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_EMISSION, new float[] { (float)mat.EmissionColor.R / 255f, (float)mat.EmissionColor.G / 255f, (float)mat.EmissionColor.B / 255f, (float)mat.EmissionColor.A / 255f });

							Gl.glEnable(Gl.GL_LIGHTING);
							if (((mat.PolyAttrib >> 0) & 0x1) == 1) Gl.glEnable(Gl.GL_LIGHT0);
							else Gl.glDisable(Gl.GL_LIGHT0);
							if (((mat.PolyAttrib >> 1) & 0x1) == 1) Gl.glEnable(Gl.GL_LIGHT1);
							else Gl.glDisable(Gl.GL_LIGHT1);
							if (((mat.PolyAttrib >> 2) & 0x1) == 1) Gl.glEnable(Gl.GL_LIGHT2);
							else Gl.glDisable(Gl.GL_LIGHT2);
							if (((mat.PolyAttrib >> 3) & 0x1) == 1) Gl.glEnable(Gl.GL_LIGHT3);
							else Gl.glDisable(Gl.GL_LIGHT3);

							if (mat.diffuseColor)
							{
								Gl.glColor4f((float)mat.DiffuseColor.R / 255f, (float)mat.DiffuseColor.G / 255f, (float)mat.DiffuseColor.B / 255f, (float)mat.DiffuseColor.A / 255f);
							}

						}
						else
						{
							Gl.glDisable(Gl.GL_LIGHTING);
							Gl.glDisable(Gl.GL_LIGHT0);
							Gl.glDisable(Gl.GL_LIGHT1);
							Gl.glDisable(Gl.GL_LIGHT2);
							Gl.glDisable(Gl.GL_LIGHT3);
							if (mat.diffuseColor)
							{
								Gl.glColor4f((float)mat.DiffuseColor.R / 255f, (float)mat.DiffuseColor.G / 255f, (float)mat.DiffuseColor.B / 255f, (float)mat.DiffuseColor.A / 255f);
							}

						}
						Gl.glEnable(Gl.GL_BLEND);
						if (mat.isEnvironmentMap)
						{
							Gl.glTexGeni(Gl.GL_S, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_SPHERE_MAP);
							Gl.glTexGeni(Gl.GL_T, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_SPHERE_MAP);
							Gl.glEnable(Gl.GL_TEXTURE_GEN_S);
							Gl.glEnable(Gl.GL_TEXTURE_GEN_T);

						}
						else
						{
							Gl.glDisable(Gl.GL_TEXTURE_GEN_S);
							Gl.glDisable(Gl.GL_TEXTURE_GEN_T);
							Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
						}

						int mode = -1;
						switch ((mat.PolyAttrib >> 4) & 0x3)
						{
							case 0:
								mode = Gl.GL_MODULATE;
								break;
							case 1:
								mode = Gl.GL_DECAL;
								break;
							case 2:
								mode = Gl.GL_MODULATE;
								break;
							case 3:
								mode = Gl.GL_MODULATE;
								break;
						}
						Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, mode);
						int cullmode = -1;

						switch (mat.PolyAttrib >> 6 & 0x03)
						{
							case 0x03: cullmode = Gl.GL_NONE; break;
							case 0x02: cullmode = Gl.GL_BACK; break;
							case 0x01: cullmode = Gl.GL_FRONT; break;
							case 0x00: cullmode = Gl.GL_FRONT_AND_BACK; break;
						}
						Gl.glCullFace(cullmode);
					}

				}
				else
				{
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
				}

				if (!gOptColoring)
				{
					Gl.glColor3f(1, 1, 1);
				}
				stackID = poly.StackID; // the first matrix used by this polygon
				Process3DCommand(poly.PolyData, Model.Materials[poly.MatId], poly.JointID, true);

			}
			writevertex = false;
		}
		/// <summary>
		/// Render model to OpenGL surface.
		/// </summary>
		public void RenderModel(float elev, float ang)
		{
			MTX44 tmp = new MTX44();
			for (var j = 0; j < Model.Polygons.Count - 1; j++)
			{
				var poly = Model.Polygons[j];
				int matid = poly.MatId;
				var mat = Model.Materials[matid];
			}

			Gl.glLineWidth(2.0F);
			float xmin = Model.boundXmin;
			float ymin = Model.boundYmin;
			float zmin = Model.boundZmin;
			float xmax = Model.boundXmax + Model.boundXmin;
			float ymax = Model.boundYmax + Model.boundYmin;
			float zmax = -Model.boundZmax + Model.boundZmin;
			
			Gl.glLineWidth(1.0F);

			////////////////////////////////////////////////////////////
			// prepare the matrix stack
			for (var i = 0; i < Model.Objects.Count; i++)
			{
				var obj = Model.Objects[i];
				var m_trans = obj.TransVect;
				float[] f = loadIdentity();
				if (obj.RestoreID != -1)
				{
					Gl.glLoadMatrixf(MatrixStack[obj.RestoreID].Floats);
				}
				if (obj.StackID != -1)
				{

					Gl.glMultMatrixf(obj.materix);
					if (obj.isBillboard)
					{
						if (!obj.isYBillboard)
						{
							Gl.glRotatef(elev, 1, 0, 0);
						}
						Gl.glRotatef(-ang, 0, 1, 0);
					}
					Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, MatrixStack[obj.StackID].Floats);
					stackID = obj.StackID; // save the last stackID
				}
				else
				{

				}
				if (obj.visible)
				{
					//light = Gl.glIsEnabled(Gl.GL_LIGHTING);
					//Gl.glDisable(Gl.GL_LIGHTING);
					//drawJoint(0.1f);
					//if (light == 1)
					//{
					//    Gl.glEnable(Gl.GL_LIGHTING);
					//}
				}

			}
			Gl.glLoadIdentity();
			
			Gl.glLoadIdentity();

			// display all polygons of the current model
			for (var i = 0; i < Model.Polygons.Count - 1; i++)
			{
				var poly = Model.Polygons[i];

				if (gOptTexture && !gOptWireFrame && g_mat)
				{
					int matid = poly.MatId;
					if (matid == -1)
					{
						Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
						if (writevertex)
						{
							mattt.Add(new ImageBrush());
						}
					}
					else
					{
						if (writevertex)
						{
							mattt.Add(matt[matid]);
						}
						var mat = Model.Materials[matid];
						
						Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

						// Convert pixel coords to normalised STs
						Gl.glMatrixMode(Gl.GL_TEXTURE);
						Gl.glLoadIdentity();
					
					scale:
						if (!mat.isEnvironmentMap)
						{
							Gl.glScalef((float)mat.scaleS / ((float)mat.width), (float)mat.scaleT / ((float)mat.height), 1.0f);
						}
					end:
						//Gl.glColor4f(1, 1, 0, 0);
						Gl.glEnable(Gl.GL_ALPHA_TEST);
						Gl.glAlphaFunc(Gl.GL_GREATER, 0f);

						if (((mat.PolyAttrib >> 0xf) & 0x1) == 1 && (((mat.PolyAttrib >> 0) & 0x1) == 0 && ((mat.PolyAttrib >> 1) & 0x1) == 0 && ((mat.PolyAttrib >> 2) & 0x1) == 0 && ((mat.PolyAttrib >> 2) & 0x1) == 0) == false)
						{
							Gl.glEnable(Gl.GL_LIGHTING);
							if (((mat.PolyAttrib >> 0) & 0x1) == 1) Gl.glEnable(Gl.GL_LIGHT0);
							else Gl.glDisable(Gl.GL_LIGHT0);
							if (((mat.PolyAttrib >> 1) & 0x1) == 1) Gl.glEnable(Gl.GL_LIGHT1);
							else Gl.glDisable(Gl.GL_LIGHT1);
							if (((mat.PolyAttrib >> 2) & 0x1) == 1) Gl.glEnable(Gl.GL_LIGHT2);
							else Gl.glDisable(Gl.GL_LIGHT2);
							if (((mat.PolyAttrib >> 3) & 0x1) == 1) Gl.glEnable(Gl.GL_LIGHT3);
							else Gl.glDisable(Gl.GL_LIGHT3);

							Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE);
							Gl.glColor4f((float)mat.DiffuseColor.R / 255f, (float)mat.DiffuseColor.G / 255f, (float)mat.DiffuseColor.B / 255f, (float)mat.DiffuseColor.A / 255f);
							Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT);
							Gl.glColor4f((float)mat.AmbientColor.R / 255f, (float)mat.AmbientColor.G / 255f, (float)mat.AmbientColor.B / 255f, (float)mat.AmbientColor.A / 255f);
							Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR);
							Gl.glColor4f((float)mat.SpecularColor.R / 255f, (float)mat.SpecularColor.G / 255f, (float)mat.SpecularColor.B / 255f, (float)mat.SpecularColor.A / 255f);
							Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION);
							Gl.glColor4f((float)mat.EmissionColor.R / 255f, (float)mat.EmissionColor.G / 255f, (float)mat.EmissionColor.B / 255f, (float)mat.EmissionColor.A / 255f);

							
							Gl.glEnable(Gl.GL_COLOR_MATERIAL);
						}
						else
						{
							Gl.glDisable(Gl.GL_LIGHTING);
							Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE);
							Gl.glColor4f((float)mat.DiffuseColor.R / 255f, (float)mat.DiffuseColor.G / 255f, (float)mat.DiffuseColor.B / 255f, (float)mat.DiffuseColor.A / 255f);
							Gl.glEnable(Gl.GL_COLOR_MATERIAL);
						}
						Gl.glEnable(Gl.GL_BLEND);
						if (mat.isEnvironmentMap)
						{
							Gl.glTexGeni(Gl.GL_S, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_SPHERE_MAP);
							Gl.glTexGeni(Gl.GL_T, Gl.GL_TEXTURE_GEN_MODE, Gl.GL_SPHERE_MAP);
							Gl.glEnable(Gl.GL_TEXTURE_GEN_S);
							Gl.glEnable(Gl.GL_TEXTURE_GEN_T);
							//Gl.glBlendFunc(Gl.GL_ONE, Gl.GL_ONE);
						}
						else
						{
							Gl.glDisable(Gl.GL_TEXTURE_GEN_S);
							Gl.glDisable(Gl.GL_TEXTURE_GEN_T);
							Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
						}

						int mode = -1;
						switch ((mat.PolyAttrib >> 4) & 0x3)
						{
							case 0:
								mode = Gl.GL_MODULATE;
								break;
							case 1:
								mode = Gl.GL_DECAL;
								break;
							case 2:
								mode = Gl.GL_MODULATE;
								break;
							case 3:
								mode = Gl.GL_MODULATE;
								break;
						}
						Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, mode);
						int cullmode = -1;
						//Gl.glEnable(Gl.GL_CULL_FACE);
						switch (mat.PolyAttrib >> 6 & 0x03)
						{
							case 0x03: cullmode = Gl.GL_NONE; break;
							case 0x02: cullmode = Gl.GL_BACK; break;
							case 0x01: cullmode = Gl.GL_FRONT; break;
							case 0x00: cullmode = Gl.GL_FRONT_AND_BACK; break;
						}
						Gl.glCullFace(cullmode);
					}

				}
				else
				{
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
				}

				if (!gOptColoring)
				{
					//Gl.glColor3f(1, 1, 1);
				}
				stackID = poly.StackID; // the first matrix used by this polygon
				Process3DCommand(poly.PolyData, Model.Materials[poly.MatId], poly.JointID, false);
			}
			writevertex = false;
		}
		public bool doJointAnimation(MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File ca, int selectedanim, bool anim, int i)
		{
			try
			{
				if (ca.Header.file_size != 0 && selectedanim != -1)
				{
					float[] s = loadIdentity();
					float[] r = loadIdentity();
					float[] t = loadIdentity();
					//float[] mt = loadIdentity();
					//scale
					float scalex = float.NaN;
					float scaley = float.NaN;
					float scalez = float.NaN;
					if (ca.JAC[selectedanim].ObjInfo[i].scale[0][0].Count != 0)
					{
						scalex = ca.JAC[selectedanim].ObjInfo[i].scale[0][0][0];// + ca.JAC[selectedanim].ObjInfo[i].scale[0][1][0]) / 2;// - ca.JAC[selectedanim].ObjInfo[i].scale[0][1][S2x[i]];//ca.JAC[selectedanim].ObjInfo[i].scale[0][0][S2x[i]];
					}
					else if (ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][0].Count != 0)
					{
						scalex = ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][0][Sx[i]];// -ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][1][Sx[i]];
					}
					if (ca.JAC[selectedanim].ObjInfo[i].scale[1][0].Count != 0)
					{
						scaley = ca.JAC[selectedanim].ObjInfo[i].scale[1][0][0];// + ca.JAC[selectedanim].ObjInfo[i].scale[1][1][0]) / 2;// -ca.JAC[selectedanim].ObjInfo[i].scale[1][1][S2y[i]];//ca.JAC[selectedanim].ObjInfo[i].scale[1][0][S2y[i]];
					}
					else if (ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[1][0].Count != 0)
					{
						scaley = ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[1][0][Sy[i]];// -ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[1][1][Sy[i]];
					}
					if (ca.JAC[selectedanim].ObjInfo[i].scale[2][0].Count != 0)
					{
						scalez = ca.JAC[selectedanim].ObjInfo[i].scale[2][0][0];// + ca.JAC[selectedanim].ObjInfo[i].scale[2][1][0]) / 2;// -ca.JAC[selectedanim].ObjInfo[i].scale[2][1][S2z[i]];//ca.JAC[selectedanim].ObjInfo[i].scale[2][0][S2z[i]];
					}
					else if (ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[2][0].Count != 0)
					{
						scalez = ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[2][0][Sz[i]];// -ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[2][1][Sz[i]];
					}
					if (!float.IsNaN(scalex) && !float.IsNaN(scaley) && !float.IsNaN(scalez))
					{
						s = scale(s, scalex, scaley, scalez);
					}
					else
					{
						//s = scale(s, Model.Objects[i].scale[0], Model.Objects[i].scale[1], Model.Objects[i].scale[2]);
					}

					/*if(ca.JAC[selectedanim].ObjInfo[i].scale[0][0].Count != 0)
					{
						//mt = scale(mt, ca.JAC[0].ObjInfo[i].scale[0][0][S2[i]], ca.JAC[0].ObjInfo[i].scale[0][1][S2[i]], ca.JAC[0].ObjInfo[i].scale[0][2][S2[i]]);
						s = scale(s, ca.JAC[selectedanim].ObjInfo[i].scale[0][0][S2[i]], ca.JAC[selectedanim].ObjInfo[i].scale[1][0][S2[i]], ca.JAC[selectedanim].ObjInfo[i].scale[2][0][S2[i]]);
						//Gl.glScalef(ca.JAC[selectedanim].ObjInfo[i].scale[0][0][S2[i]], ca.JAC[selectedanim].ObjInfo[i].scale[1][0][S2[i]], ca.JAC[selectedanim].ObjInfo[i].scale[2][0][S2[i]]);
					}*/
					try
					{
						if (ca.JAC[selectedanim].ObjInfo[i].rotate.Count != 0)
						{
							if ((((int)ca.JAC[selectedanim].ObjInfo[i].rotate[0]) >> 15 & 0x1) == 1)
							{
								int pvneg = ca.JAC[selectedanim].JointData[(((int)ca.JAC[selectedanim].ObjInfo[i].rotate[0]) & 0x7fff) * 6];//Utils.Read2BytesAsInt16(ca.JAC[selectedanim].JointData, (int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 6 + 0);
								int a = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].JointData, (((int)ca.JAC[selectedanim].ObjInfo[i].rotate[0]) & 0x7fff) * 6 + 2);
								int b = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].JointData, (((int)ca.JAC[selectedanim].ObjInfo[i].rotate[0]) & 0x7fff) * 6 + 4);
								a = sign(a, 16);
								b = sign(b, 16);
								//mt = multMatrix(mt, Nsbmd.mtxPivot(new float[] { (float)a / 4096f, (float)b / 4096f }, (pvneg >> 0 & 0x0f), (pvneg >> 4 & 0x0f)));
								//Gl.glMultMatrixf(Nsbmd.mtxPivot(new float[] { (float)a / 4096f, (float)b / 4096f }, (pvneg >> 0 & 0x0f), (pvneg >> 4 & 0x0f)));
								r = NSBMD.mtxPivot(new float[] { (float)a / 4096f, (float)b / 4096f }, (pvneg >> 0 & 0x0f), (pvneg >> 4 & 0x0f));//multMatrix(r, Nsbmd.mtxPivot(new float[] { (float)a / 4096f, (float)b / 4096f }, (pvneg >> 0 & 0x0f), (pvneg >> 4 & 0x0f)));
							}
							else
							{
								int x = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].RotationData, (((int)ca.JAC[selectedanim].ObjInfo[i].rotate[0]) & 0x7fff) * 10 + 2);
								x = sign(x, 16);
								int y = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].RotationData, (((int)ca.JAC[selectedanim].ObjInfo[i].rotate[0]) & 0x7fff) * 10 + 4);
								y = sign(y, 16);
								int z = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].RotationData, (((int)ca.JAC[selectedanim].ObjInfo[i].rotate[0]) & 0x7fff) * 10 + 6);
								z = sign(z, 16);
								OpenTK.Matrix4 X = OpenTK.Matrix4.CreateRotationX((float)x / 131072F);//((float)x * (float)Math.PI) / 32768F);
								OpenTK.Matrix4 Y = OpenTK.Matrix4.CreateRotationY((float)y / 131072F);//((float)y * (float)Math.PI) / 32768F);
								OpenTK.Matrix4 Z = OpenTK.Matrix4.CreateRotationZ((float)z / 131072F);//((float)z * (float)Math.PI) / 32768F);
								OpenTK.Matrix4 full = OpenTK.Matrix4.Identity;
								full = OpenTK.Matrix4.Mult(full, X);
								full = OpenTK.Matrix4.Mult(full, Y);
								full = OpenTK.Matrix4.Mult(full, Z);
							}
						}
						else if (ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0].Count != 0)
						{
							//Gl.glMultMatrixf(Nsbmd.mtxPivot(new float[]{ca.JAC[0].ObjInfo[i].rotate_keyframes[1][R[i]],ca.JAC[0].ObjInfo[i].rotate_keyframes[0][R[i]]}, obj.Pivot, obj.Neg));
							//Gl.glRotatef(ca.JAC[0].ObjInfo[i].rotate_keyframes[0][R[i]], 0,(ca.JAC[0].ObjInfo[i].rotate_keyframes[1][R[i]] == 1 ? 1 : 0),(ca.JAC[0].ObjInfo[i].rotate_keyframes[1][R[i]] == 0 ? 1 : 0));
							if (ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[1][R[i]] == 1)
							{
								int pvneg = ca.JAC[selectedanim].JointData[(int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 6];//Utils.Read2BytesAsInt16(ca.JAC[selectedanim].JointData, (int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 6 + 0);
								int a = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].JointData, (int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 6 + 2);
								int b = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].JointData, (int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 6 + 4);
								a = sign(a, 16);
								b = sign(b, 16);
								//mt = multMatrix(mt, Nsbmd.mtxPivot(new float[] { (float)a / 4096f, (float)b / 4096f }, (pvneg >> 0 & 0x0f), (pvneg >> 4 & 0x0f)));
								//Gl.glMultMatrixf(Nsbmd.mtxPivot(new float[] { (float)a / 4096f, (float)b / 4096f }, (pvneg >> 0 & 0x0f), (pvneg >> 4 & 0x0f)));
								r = NSBMD.mtxPivot(new float[] { (float)a / 4096f, (float)b / 4096f }, (pvneg >> 0 & 0x0f), (pvneg >> 4 & 0x0f));//multMatrix(r, Nsbmd.mtxPivot(new float[] { (float)a / 4096f, (float)b / 4096f }, (pvneg >> 0 & 0x0f), (pvneg >> 4 & 0x0f)));
							}
							else
							{
								float param = ca.JAC[selectedanim].RotationData[(int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 10];
								//float u = (float)Math.Pow(2.0f, param) * (float)180 / 131072F;
								int x = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].RotationData, (int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 10 + 2);
								x = sign(x, 16);
								int y = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].RotationData, (int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 10 + 4);
								y = sign(y, 16);
								int z = Utils.Read2BytesAsInt16(ca.JAC[selectedanim].RotationData, (int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 10 + 6);
								z = sign(z, 16);
								OpenTK.Matrix4 X = OpenTK.Matrix4.CreateRotationX((float)x / 131072F);//((float)x * (float)Math.PI) / 32768F);
								OpenTK.Matrix4 Y = OpenTK.Matrix4.CreateRotationY((float)y / 131072F);//((float)y * (float)Math.PI) / 32768F);
								OpenTK.Matrix4 Z = OpenTK.Matrix4.CreateRotationZ((float)z / 131072F);//((float)z * (float)Math.PI) / 32768F);
								OpenTK.Matrix4 full = OpenTK.Matrix4.Identity;
								full = OpenTK.Matrix4.Mult(full, X);
								full = OpenTK.Matrix4.Mult(full, Y);
								full = OpenTK.Matrix4.Mult(full, Z);
								/*int x = (param) & 0xFF;
								if ((x & 0x200) != 0) x |= -256;
								int y = (param >> 8) & 0xFF;
								if ((y & 0x200) != 0) y |= -256;
								int z = (param >> 16) & 0xFF;
								if ((z & 0x200) != 0) z |= -256;*/
								//Gl.glRotatef((float)x / 32768F * 180F, 1, 0, 0);
								//Gl.glRotatef((float)y / 32768F * 180F, 0, 1, 0);
								//Gl.glRotatef((float)z / 32768F * 180F, 0, 0, 1);
								//param /= 4096f;
								//float un = 2048f;//(float)Utils.Read4BytesAsInt32(ca.JAC[selectedanim].RotationData, (int)ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0][R[i]] * 10 + 6) / 4096f;
								//Gl.glRotatef(x / un, 1, 0, 0);//((float)x * (float)Math.PI) / 180f, 1, 0, 0);
								//Gl.glRotatef(y / un, 0, 1, 0);//((float)y * (float)Math.PI) / 180f, 0, 1, 0);
								//Gl.glRotatef(z / un, 0, 0, 1);//((float)z * (float)Math.PI) / 180f, 0, 0, 1);
								//mt = rotate(mt, x, y, z);
								//Gl.glMultMatrixf(full.ToFloat());
							}
						}
						else
						{
							//r = multMatrix(r,  Model.Objects[i].rotate_mtx);
						}
					}
					catch { }

					float transx = float.NaN;
					float transy = float.NaN;
					float transz = float.NaN;
					if (ca.JAC[selectedanim].ObjInfo[i].translate[0].Count != 0)
					{
						transx = ca.JAC[selectedanim].ObjInfo[i].translate[0][0];// Model.modelScale;
					}
					else if (ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[0].Count != 0)
					{
						transx = ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[0][Tx[i]];// Model.modelScale;
					}
					if (ca.JAC[selectedanim].ObjInfo[i].translate[1].Count != 0)
					{
						transy = ca.JAC[selectedanim].ObjInfo[i].translate[1][0];// Model.modelScale;
					}
					else if (ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[1].Count != 0)
					{
						transy = ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[1][Ty[i]];// Model.modelScale;
					}
					if (ca.JAC[selectedanim].ObjInfo[i].translate[2].Count != 0)
					{
						transz = ca.JAC[selectedanim].ObjInfo[i].translate[2][0];// Model.modelScale;
					}
					else if (ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[2].Count != 0)
					{
						transz = ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[2][Tz[i]];// Model.modelScale;
					}
					if (!float.IsNaN(transx) && !float.IsNaN(transy) && !float.IsNaN(transz))
					{
						t = translate(t, transx / Model.modelScale/* - (Model.Objects[i].X)*/, transy / Model.modelScale/* - (Model.Objects[i].Y)*/, transz / Model.modelScale/* - (Model.Objects[i].Z)*/);
					}
					else
					{
						//t = translate(t, Model.Objects[i].X, Model.Objects[i].Y, Model.Objects[i].Z);
					}



					//if(ca.JAC[selectedanim].ObjInfo[i].translate[0].Count != 0)
					//{
					//mt = translate(mt, ca.JAC[0].ObjInfo[i].translate[0][T2[i]], ca.JAC[0].ObjInfo[i].translate_keyframes[1][T2[i]], ca.JAC[0].ObjInfo[i].translate_keyframes[2][T2[i]]);
					//Gl.glTranslatef(ca.JAC[selectedanim].ObjInfo[i].translate[0][T2[i]] / Model.modelScale, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[1][T2[i]] / Model.modelScale, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[2][T2[i]] / Model.modelScale);
					//    t = translate(t,ca.JAC[selectedanim].ObjInfo[i].translate[0][T2[i]] / Model.modelScale, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[1][T2[i]] / Model.modelScale, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[2][T2[i]] / Model.modelScale);
					//}
					float[] m = loadIdentity();
					//Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, m);
					m = multMatrix(m, t);
					//m = translate(m, Model.Objects[i].X, Model.Objects[i].Y, Model.Objects[i].Z);
					//if (Model.Objects[i].IsRotated)
					//{
					//float[] rt = loadIdentity();
					//rt = multMatrix(rt, r);
					//rt = multMatrix(rt, Model.Objects[i].rotate_mtx);
					m = multMatrix(m, r);
					// m = multMatrix(m, Model.Objects[i].rotate_mtx);

					//}
					//else
					//{
					//    m = multMatrix(m, r);
					//} 
					m = multMatrix(m, s);
					//m = scale(m, Model.Objects[i].scale[0], Model.Objects[i].scale[1], Model.Objects[i].scale[2]);
					Gl.glMultMatrixf(m);
					//s = loadIdentity();
					//r = loadIdentity();
					//t = loadIdentity();
					//Gl.glMultMatrixf(mt);
					//mt = loadIdentity();
					//if(ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][0].Count != 0)
					// {
					//mt = scale(mt, ca.JAC[0].ObjInfo[i].scale_keyframes[0][0][S[i]], ca.JAC[0].ObjInfo[i].scale_keyframes[0][1][S[i]], ca.JAC[0].ObjInfo[i].scale_keyframes[0][2][S[i]]);
					//Gl.glScalef((ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][0][S[i]] - ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][1][S[i]]), (ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[1][0][S[i]] - ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[1][1][S[i]]), (ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[2][0][S[i]] - ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[2][1][S[i]]));
					//s = scale(s, (ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][0][S[i]] - ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][1][S[i]]), (ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[1][0][S[i]] - ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[1][1][S[i]]), (ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[2][0][S[i]] - ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[2][1][S[i]]));
					//}

					//if(ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[0].Count != 0)
					//{
					//mt = translate(mt, ca.JAC[0].ObjInfo[i].translate_keyframes[0][T[i]], ca.JAC[0].ObjInfo[i].translate_keyframes[1][T[i]], ca.JAC[0].ObjInfo[i].translate_keyframes[2][T[i]]);
					//Gl.glTranslatef(ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[0][T[i]] / Model.modelScale, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[1][T[i]] / Model.modelScale, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[2][T[i]] / Model.modelScale);
					//   t = translate(t, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[0][T[i]] / Model.modelScale, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[1][T[i]] / Model.modelScale, ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[2][T[i]] / Model.modelScale);
					//}
					// m = loadIdentity();
					//m = multMatrix(m, t);
					//m = multMatrix(m, r);
					//m = multMatrix(m, s);
					//Gl.glMultMatrixf(m);
					if (anim)
					{
						if (Tx[i] == ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[0].Count - 1)
						{
							Tx[i] = 0;
						}
						else
						{
							Tx[i]++;
						}
						if (Ty[i] == ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[1].Count - 1)
						{
							Ty[i] = 0;
						}
						else
						{
							Ty[i]++;
						}
						if (Tz[i] == ca.JAC[selectedanim].ObjInfo[i].translate_keyframes[2].Count - 1)
						{
							Tz[i] = 0;
						}
						else
						{
							Tz[i]++;
						}
						if (R[i] == ca.JAC[selectedanim].ObjInfo[i].rotate_keyframes[0].Count - 1)
						{
							R[i] = 0;
						}
						else
						{
							R[i]++;
						}
						if (Sx[i] == ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[0][0].Count - 1)
						{
							Sx[i] = 0;
						}
						else
						{
							Sx[i]++;
						}
						if (Sy[i] == ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[1][0].Count - 1)
						{
							Sy[i] = 0;
						}
						else
						{
							Sy[i]++;
						}
						if (Sz[i] == ca.JAC[selectedanim].ObjInfo[i].scale_keyframes[2][0].Count - 1)
						{
							Sz[i] = 0;
						}
						else
						{
							Sz[i]++;
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}
			catch { return true; }
		}
		public static float[] translate(float[] a, float x, float y, float z)
		{
			float[] b = loadIdentity();
			b[12] = x;
			b[13] = y;
			b[14] = z;
			return multMatrix(a, b);
		}
		public static float[] loadIdentity()
		{
			float[] a = new float[16];
			a[0] = 1.0F;
			a[5] = 1.0F;
			a[10] = 1.0F;
			a[15] = 1.0F;
			return a;
		}
		public static float[] scale(float[] a, float x, float y, float z)
		{
			float[] b = loadIdentity();
			b[0] = x;
			b[5] = y;
			b[10] = z;
			return multMatrix(a, b);
		}

		private void setColor(int drawMode, float[] color)
		{
			/*bool checkJoint = true;
			bool selJoint = true;
			if(jointId < Frame.jointList.length)
			{
				checkJoint = Frame.jointList[jointId].isChecked();
				selJoint = Frame.jointList[jointId].isSelected();
			}
			boolean checkStack = visible[stackId];
			boolean checkPoly = Frame.polygonList[polyId].isChecked();
			boolean checkMat = true;
			boolean selMat = true;
			if(matId < Frame.materialList.length)
			{
				checkMat = Frame.materialList[matId].isChecked();
				selMat = Frame.materialList[matId].isSelected();
			}
			boolean selPoly = Frame.polygonList[polyId].isSelected();*/
			switch (drawMode)
			{
				case 0: // '\0'
				default:
					break;

				case 1: // '\001'
					color = lineColorOff;
					/*if(selPoly)
					{
						color = lineColorOn;
						if(Frame.points)
							color = lineColorEdit;
					}*/
					break;

				case 2: // '\002'
					color = pointColorOff;
					/*if(selPoly)
						color = pointColorOn;*/
					break;

				case 3: // '\003'
					color = jointColor;
					/*if(selJoint)
						color = jointColorOn;*/
					break;

				case 4: // '\004'
					color = linkColor;
					/*if(selJoint)
						color = linkColorOn;*/
					break;
			}
			float alpha = color[3];
			//if(drawMode == 0 && !checkPoly || (drawMode == 3 || drawMode == 4) && !checkJoint)
			//    alpha = 0.2F;
			//if(drawMode == 0 && !checkStack)
			//    alpha = 0.0F;
			Gl.glColor4f(color[0], color[1], color[2], alpha);
		}

		private float[] baseColor = {
        1.0F, 1.0F, 1.0F, 1.0F
    };
		private float[] shadeColor = {
        0.3F, 0.3F, 0.3F, 1.0F
    };
		private float[] selColor = {
        1.0F, 0.0F, 0.0F, 1.0F
    };
		private float[] lineColorOff = {
        0.0F, 0.02745098F, 0.5686275F, 1.0F
    };
		private float[] lineColorOn = {
        0.2627451F, 1.0F, 0.6392157F, 1.0F
    };
		private float[] lineColorEdit = {
        0.3921569F, 0.8627451F, 1.0F, 1.0F
    };
		private float[] pointColorOff = {
        0.3921569F, 0.1960784F, 0.4313726F, 1.0F
    };
		private float[] pointColorOn = {
        0.7843137F, 0.0F, 0.7843137F, 1.0F
    };
		private float[] pointColorEdit = {
        0.7843137F, 0.0F, 0.7843137F, 1.0F
    };
		private float[] jointColor = {
        0.0F, 0.4F, 0.0F, 1.0F
    };
		private float[] jointColorOn = {
        0.4F, 0.0F, 0.0F, 1.0F
    };
		private float[] linkColor = {
        0.0F, 0.0F, 0.8F, 0.8F
    };
		private float[] linkColorOn = {
        0.8F, 0.0F, 0.0F, 0.8F
    };
		public static float[] multMatrix(float[] a, float[] b)
		{
			float[] c = new float[16];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					c[(i << 2) + j] = 0.0F;
					for (int k = 0; k < 4; k++)
						c[(i << 2) + j] += a[(k << 2) + j] * b[(i << 2) + k];

				}

			}

			return c;
		}
		public static float[] loadMatrix(float[] fmatrix, int stack)
		{
			float[] a = new float[16];
			a[0] = 1.0F;
			a[5] = 1.0F;
			a[10] = 1.0F;
			a[15] = 1.0F;
			for (int i = 0; i < a.Length; i++)
				a[i] = fmatrix[stack * 16 + i];

			return a;
		}

		public float[] pullVector(float[] fmatrix, int offset)
		{
			float[] cmatrix = new float[16];
			for (int i = 0; i < cmatrix.Length; i++)
				cmatrix[i] = fmatrix[offset + i];

			return multVector(cmatrix, new float[] {
            0.0F, 0.0F, 0.0F
        });
		}

		public float[] multVector(float[] cmatrix, float[] vtxState)
		{
			float[] vtxTrans = new float[3];
			for (int i = 0; i < 3; i++)
			{
				float c0 = vtxState[0] * cmatrix[0 + i];
				float c1 = vtxState[1] * cmatrix[4 + i];
				float c2 = vtxState[2] * cmatrix[8 + i];
				float c3 = cmatrix[12 + i];
				vtxTrans[i] = c0 + c1 + c2 + c3;
			}

			return vtxTrans;
		}

		// Private Methods (7) 

		/// <summary>
		/// Convert texel.
		/// </summary>
		private bool convert_4x4texel(uint[] tex, int width, int height, UInt16[] data, RGBA[] pal, RGBA[] rgbaOut)
		{
			int w = width / 4;
			int h = height / 4;

			// traverse 'w x h blocks' of 4x4-texel
			for (int y = 0; y < h; y++)
				for (int x = 0; x < w; x++)
				{
					int index = y * w + x;
					UInt32 t = tex[index];
					UInt16 d = data[index];
					UInt16 addr = (ushort)(d & 0x3fff);
					UInt16 mode = (ushort)((d >> 14) & 3);

					// traverse every texel in the 4x4 texels
					for (int r = 0; r < 4; r++)
						for (int c = 0; c < 4; c++)
						{
							int texel = (int)((t >> ((r * 4 + c) * 2)) & 3);
							RGBA pixel = rgbaOut[(y * 4 + r) * width + (x * 4 + c)];

							switch (mode)
							{
								case 0:
									pixel = pal[(addr << 1) + texel];
									if (texel == 3) pixel = RGBA.Transparent; // make it transparent, alpha = 0
									break;
								case 2:
									pixel = pal[(addr << 1) + texel];
									break;
								case 1:
									switch (texel)
									{
										case 0:
										case 1:
											pixel = pal[(addr << 1) + texel];
											break;
										case 2:
											pixel.R = (byte)((pal[(addr << 1)].R + pal[(addr << 1) + 1].R) / 2L);
											pixel.G = (byte)((pal[(addr << 1)].G + pal[(addr << 1) + 1].G) / 2L);
											pixel.B = (byte)((pal[(addr << 1)].B + pal[(addr << 1) + 1].B) / 2L);
											pixel.A = 0xff;
											break;
										case 3:
											pixel = RGBA.Transparent; // make it transparent, alpha = 0
											break;
									}
									break;
								case 3:
									switch (texel)
									{
										case 0:
										case 1:
											pixel = pal[(addr << 1) + texel];
											break;
										case 2:
											pixel.R = (byte)((pal[(addr << 1)].R * 5L + pal[(addr << 1) + 1].R * 3L) / 8);
											pixel.G = (byte)((pal[(addr << 1)].G * 5L + pal[(addr << 1) + 1].G * 3L) / 8);
											pixel.B = (byte)((pal[(addr << 1)].B * 5L + pal[(addr << 1) + 1].B * 3L) / 8);
											pixel.A = 0xff;
											break;
										case 3:
											pixel.R = (byte)((pal[(addr << 1)].R * 3L + pal[(addr << 1) + 1].R * 5L) / 8);
											pixel.G = (byte)((pal[(addr << 1)].G * 3L + pal[(addr << 1) + 1].G * 5L) / 8);
											pixel.B = (byte)((pal[(addr << 1)].B * 3L + pal[(addr << 1) + 1].B * 5L) / 8);
											pixel.A = 0xff;
											break;
									}
									break;
							}
							rgbaOut[(y * 4 + r) * width + (x * 4 + c)] = pixel;
						}
				}
			return true;
		}

		/// <summary>
		/// Convert texel (wrapper for type safety issues).
		/// </summary>
		private void convert_4x4texel_b(byte[] tex, int width, int height, byte[] data, RGBA[] pal, RGBA[] rgbaOut)
		{
			var list1 = new List<uint>();
			for (int i = 0; i < (tex.Length + 1) / 4; ++i)
				list1.Add(Utils.Read4BytesAsUInt32(tex, i * 4));

			var list2 = new List<UInt16>();
			for (int i = 0; i < (data.Length + 1) / 2; ++i)
				list2.Add(Utils.Read2BytesAsUInt16(data, i * 2));
			var b = convert_4x4texel(list1.ToArray(), width, height, list2.ToArray(), pal, rgbaOut);
		}


		/// <summary>
		/// Make texture for model.
		/// </summary>
		/// <param name="mod">NSBMD Model</param>
		private void MakeTexture(NSBMDModel mod)
		{
			Gl.glMatrixMode(Gl.GL_TEXTURE_MATRIX);
			Gl.glLoadIdentity();


			Console.WriteLine("DEBUG: making texture for model '{0}'...", mod.Name);

			for (int i = 0; i < mod.Materials.Count - 1; i++)
			{
				try
				{
					if (mod.Materials[i].format == 0) // format 0 is no texture
					{
						matt.Add(new ImageBrush());
                        continue;
					}
					var mat = mod.Materials[i];
					if (mat == null || (mat.paldata == null && mat.format != 7))
					{
						matt.Add(new ImageBrush());
						continue;
					}
					int pixelnum = mat.width * mat.height;

					var image = new RGBA[pixelnum];


					switch (mat.format)
					{
						// No Texture
						case 0:
                            //puts( "ERROR: format 0" );
							mattt.Add(new ImageBrush());
							continue;
						// A3I5 Translucent Texture (3bit Alpha, 5bit Color Index)
						case 1:
							for (int j = 0; j < pixelnum; j++)
							{
								int index = mat.texdata[j] & 0x1f;
								int alpha = (mat.texdata[j] >> 5);// & 7;
								alpha = ((alpha * 4) + (alpha / 2)) * 8;// << 3;
								image[j] = mat.paldata[index];
								image[j].A = (byte)alpha;
							}

							break;
						// 4-Color Palette Texture
						case 2:
							if (mat.color0 != 0) mat.paldata[0] = RGBA.Transparent; // made palette entry 0 transparent
							for (int j = 0; j < pixelnum; j++)
							{
								uint index = mat.texdata[j / 4];
								index = (index >> ((j % 4) << 1)) & 3;
								image[j] = mat.paldata[index];
							}
							break;
						// 16-Color Palette Texture
						case 3:
							if (mat.color0 != 0) mat.paldata[0] = RGBA.Transparent; // made palette entry 0 transparent
							for (int j = 0; j < pixelnum; j++)
							{
								var matindex = j / 2;
								if (mat.texdata.Length < matindex)
									continue;
								int index = mat.texdata[matindex];
								index = (index >> ((j % 2) << 2)) & 0x0f;
								if (mat.paldata == null)
									continue;
								if (index < 0 || index >= mat.paldata.Length)
									continue;
								if (j < 0 || j >= pixelnum)
									continue;
								image[j] = mat.paldata[index];
							}
							break;
						// 256-Color Palette Texture
						case 4:
							if (mat.color0 != 0) mat.paldata[0] = RGBA.Transparent; // made palette entry 0 transparent
							// made palette entry 0 transparent
							for (int j = 0; j < pixelnum; j++)
							{
								image[j] = mat.paldata[mat.texdata[j]];
							}
							break;
						// 4x4-Texel Compressed Texture
						case 5:
							convert_4x4texel_b(mat.texdata, mat.width, mat.height, mat.spdata, mat.paldata, image);
							break;
						// A5I3 Translucent Texture (5bit Alpha, 3bit Color Index)
						case 6:
							for (int j = 0; j < pixelnum; j++)
							{
								int index = mat.texdata[j] & 0x7;
								int alpha = (mat.texdata[j] >> 3);
								alpha *= 8; //((alpha * 4) + (alpha / 2)) << 3;
								image[j] = mat.paldata[index];
								image[j].A = (byte)alpha;
							}
							break;
						// Direct Color Texture
						case 7:
							for (int j = 0; j < pixelnum; j++)
							{
								//UInt16 p = (ushort)(mat.texdata[j * 2] + (mat.texdata[j * 2 + 1] << 8));
								//image[j].R = (byte)(((p >> 0) & 0x1f) << 3);
								//image[j].G = (byte)(((p >> 5) & 0x1f) << 3);
								//image[j].B = (byte)(((p >> 10) & 0x1f) << 3);
								//image[j].A = (byte)(((p & 0x8000) != 0) ? 0xff : 0);
								image[j] = RGBA.fromColor(Tinke.Convertir.BGR555(mat.texdata[j * 2], mat.texdata[j * 2 + 1]));
								UInt16 p = (ushort)(mat.texdata[j * 2] + (mat.texdata[j * 2 + 1] << 8));
								image[j].A = (byte)(((p & 0x8000) != 0) ? 0xff : 0);
							}
							break;
					}

					Console.WriteLine("convert matid = {0}", i);
					Console.WriteLine("\ttex '{0}': {1} [{2},{3}] texsize = {4}", mat.texname, TEXTURE_FORMATS[mat.format], mat.width,
									  mat.height, mat.texsize);
					Console.WriteLine("\tpal '{0}': pixelnum = {1}, repeat = {2}", mat.palname, pixelnum, mat.repeat);

					var imageBytesList = new List<byte>();
					System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(mat.width, mat.height);
					for (int k = 0; k < image.Length; ++k)
					{
						bitmap.SetPixel(k - ((k / (mat.width)) * (mat.width)), k / (mat.width), image[k].ToColor());
						imageBytesList.Add(image[k].R);
						imageBytesList.Add(image[k].G);
						imageBytesList.Add(image[k].B);
					    imageBytesList.Add((image[k].A));

					}
					var imageBytes = imageBytesList.ToArray();
					if (mat.flipS == 1 && mat.flipT == 1)
					{
						System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(mat.width * 2, mat.height * 2);
						using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap2))
						{
							g.DrawImage(bitmap, 0, 0);
							System.Drawing.Bitmap tmp = bitmap;
							tmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
							g.DrawImage(tmp, mat.width, 0);
							tmp = bitmap;
							tmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
							g.DrawImage(tmp, mat.width, mat.height);
							tmp = bitmap;
							tmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
							g.DrawImage(tmp, 0, mat.height);
						}
						bitmap = bitmap2;
					}
					else if (mat.flipS == 1)
					{
						//br.TileMode = System.Windows.Media.TileMode.FlipX;
						System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(mat.width * 2, mat.height);
						using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap2))
						{
							g.DrawImage(bitmap, 0, 0);
							System.Drawing.Bitmap tmp = bitmap;
							tmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
							g.DrawImage(tmp, mat.width, 0);
						}
						bitmap = bitmap2;
					}
					else if (mat.flipT == 1)
					{
						//br.TileMode = System.Windows.Media.TileMode.FlipY;
						System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(mat.width, mat.height * 2);
						using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap2))
						{
							g.DrawImage(bitmap, 0, 0);
							System.Drawing.Bitmap tmp = bitmap;
							tmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
							g.DrawImage(tmp, 0, mat.height);
						}
						bitmap = bitmap2;
					}
					else if (mat.repeatS == 1 || mat.repeatT == 1)
					{
						//br.TileMode = System.Windows.Media.TileMode.Tile;
					}
					else
					{
						//br.TileMode = System.Windows.Media.TileMode.None;
					}
					ImageBrush br = new ImageBrush(CreateBitmapSourceFromBitmap(bitmap));
					br.Viewbox = new Rect(0, 0, br.ImageSource.Width, br.ImageSource.Height);
					br.ViewboxUnits = BrushMappingMode.Absolute;
					br.Viewport = new Rect(0, 0, 1, 1);
					br.ViewportUnits = BrushMappingMode.Absolute;
					br.Stretch = Stretch.None;

					//br.ImageSource = CreateBitmapSourceFromBitmap(bitmap);
					br.Opacity = (double)(((mat.Alpha + 1) * 8) - 1) / 1.0d;
					matt.Add(br);
					//ttt
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, i + 1 + matstart);
					Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, mat.width, mat.height, 0, Gl.GL_RGBA,
									Gl.GL_UNSIGNED_BYTE,
									imageBytes);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
					Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);

					if (mat.flipS == 1 && mat.repeatS == 1)
					{
						Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_MIRRORED_REPEAT);
					}
					else if (mat.repeatS == 1)
					{
						Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
					}
					else
					{
						Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
					}
					if (mat.flipT == 1 && mat.repeatT == 1)
					{
						Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_MIRRORED_REPEAT);
					}
					else if (mat.repeatT == 1)
					{
						Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
					}
					else
					{
						Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
					}
				}
				catch
				{
                    matt.Add(new ImageBrush());
                }
			}

		}
		/// <summary>
		/// Make texture for model.
		/// </summary>
		/// <param name="mod">NSBMD Model</param>
		private void MakeTexture(int i, NSBMDMaterial m)
		{
			try
			{
				if (m.format == 0) // format 0 is no texture
				{
					//matt.Add(new System.Windows.Media.ImageBrush());
					return;
				}
				var mat = m;
				if (mat == null || (mat.paldata == null && mat.format != 7))
				{
					//matt.Add(new System.Windows.Media.ImageBrush());
					return;
				}
				int pixelnum = mat.width * mat.height;

				var image = new RGBA[pixelnum];


				switch (mat.format)
				{
					// No Texture
					case 0:
						//puts( "ERROR: format 0" );
						//mattt.Add(new System.Windows.Media.ImageBrush());
						return;
					// A3I5 Translucent Texture (3bit Alpha, 5bit Color Index)
					case 1:
						for (int j = 0; j < pixelnum; j++)
						{
							int index = mat.texdata[j] & 0x1f;
							int alpha = (mat.texdata[j] >> 5);// & 7;
							alpha = ((alpha * 4) + (alpha / 2)) * 8;// << 3;
							image[j] = mat.paldata[index];
							image[j].A = (byte)alpha;
						}

						break;
					// 4-Color Palette Texture
					case 2:
						if (mat.color0 != 0) mat.paldata[0] = RGBA.Transparent; // made palette entry 0 transparent
						for (int j = 0; j < pixelnum; j++)
						{
							uint index = mat.texdata[j / 4];
							index = (index >> ((j % 4) << 1)) & 3;
							image[j] = mat.paldata[index];
						}
						break;
					// 16-Color Palette Texture
					case 3:
						if (mat.color0 != 0) mat.paldata[0] = RGBA.Transparent; // made palette entry 0 transparent
						for (int j = 0; j < pixelnum; j++)
						{
							var matindex = j / 2;
							if (mat.texdata.Length < matindex)
								continue;
							int index = mat.texdata[matindex];
							index = (index >> ((j % 2) << 2)) & 0x0f;
							if (mat.paldata == null)
								continue;
							if (index < 0 || index >= mat.paldata.Length)
								continue;
							if (j < 0 || j >= pixelnum)
								continue;
							image[j] = mat.paldata[index];
						}
						break;
					// 256-Color Palette Texture
					case 4:
						if (mat.color0 != 0) mat.paldata[0] = RGBA.Transparent; // made palette entry 0 transparent
						// made palette entry 0 transparent
						for (int j = 0; j < pixelnum; j++)
						{
							image[j] = mat.paldata[mat.texdata[j]];
						}
						break;
					// 4x4-Texel Compressed Texture
					case 5:
						convert_4x4texel_b(mat.texdata, mat.width, mat.height, mat.spdata, mat.paldata, image);
						break;
					// A5I3 Translucent Texture (5bit Alpha, 3bit Color Index)
					case 6:
						for (int j = 0; j < pixelnum; j++)
						{
							int index = mat.texdata[j] & 0x7;
							int alpha = (mat.texdata[j] >> 3);
							alpha *= 8; //((alpha * 4) + (alpha / 2)) << 3;
							image[j] = mat.paldata[index];
							image[j].A = (byte)alpha;
						}
						break;
					// Direct Color Texture
					case 7:
						for (int j = 0; j < pixelnum; j++)
						{
							//UInt16 p = (ushort)(mat.texdata[j * 2] + (mat.texdata[j * 2 + 1] << 8));
							//image[j].R = (byte)(((p >> 0) & 0x1f) << 3);
							//image[j].G = (byte)(((p >> 5) & 0x1f) << 3);
							//image[j].B = (byte)(((p >> 10) & 0x1f) << 3);
							//image[j].A = (byte)(((p & 0x8000) != 0) ? 0xff : 0);
							image[j] = RGBA.fromColor(Tinke.Convertir.BGR555(mat.texdata[j * 2], mat.texdata[j * 2 + 1]));
							UInt16 p = (ushort)(mat.texdata[j * 2] + (mat.texdata[j * 2 + 1] << 8));
							image[j].A = (byte)(((p & 0x8000) != 0) ? 0xff : 0);
						}
						break;
				}

				Console.WriteLine("convert matid = {0}", i);
				Console.WriteLine("\ttex '{0}': {1} [{2},{3}] texsize = {4}", mat.texname, TEXTURE_FORMATS[mat.format], mat.width,
								  mat.height, mat.texsize);
				Console.WriteLine("\tpal '{0}': pixelnum = {1}, repeat = {2}", mat.palname, pixelnum, mat.repeat);

				var imageBytesList = new List<byte>();
				//System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(mat.width, mat.height);
				for (int k = 0; k < image.Length; ++k)
				{
					//bitmap.SetPixel(k - ((k / (mat.width)) * (mat.width)), k / (mat.width), image[k].ToColor());
					imageBytesList.Add(image[k].R);
					imageBytesList.Add(image[k].G);
					imageBytesList.Add(image[k].B);
					//if (image[k].A != 0)
					//{
					//	imageBytesList.Add((byte)(image[k].A - (255 - (((mat.Alpha + 1) * 8) - 1))));
					//}
					//else
					//{
						imageBytesList.Add((byte)(image[k].A));
					//}
				}

				var imageBytes = imageBytesList.ToArray();

				//ttt
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, i + 1 + matstart);
				Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, mat.width, mat.height, 0, Gl.GL_RGBA,
								Gl.GL_UNSIGNED_BYTE,
								imageBytes);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);

				if (mat.flipS == 1 && mat.repeatS == 1)
				{
					Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_MIRRORED_REPEAT);
				}
				else if (mat.repeatS == 1)
				{
					Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
				}
				else
				{
					Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
				}
				if (mat.flipT == 1 && mat.repeatT == 1)
				{
					Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_MIRRORED_REPEAT);
				}
				else if (mat.repeatT == 1)
				{
					Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
				}
				else
				{
					Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
				}
			}
			catch
			{
                //matt.Add(new System.Windows.Media.ImageBrush());
            }
		}
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		public static System.Windows.Media.Imaging.BitmapSource CreateBitmapSourceFromBitmap(System.Drawing.Bitmap bitmap)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap");

			IntPtr hBitmap = bitmap.GetHbitmap();

			try
			{
				return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
					hBitmap,
					IntPtr.Zero,
					System.Windows.Int32Rect.Empty,
					System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
			}
			finally
			{
				DeleteObject(hBitmap);
			}

		}
		private System.Windows.Media.Imaging.BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap dImg)
		{
			MemoryStream ms = new MemoryStream();
			dImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
			bImg.BeginInit();
			bImg.StreamSource = ms;
			bImg.EndInit();
			return bImg;
		}

		/// <summary>
		/// Process polygon 3d commands.
		/// </summary>
		/// <param name="polydata">Data of specific polygon.</param>
		private void Process3DCommand(byte[] polydata, NSBMDMaterial m, int jointID, bool color)
		{
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			MaterialHelper.CreateMaterial(new ImageBrush());

			md.Add(new MeshBuilder());
			IList<Point3D> poi = new List<Point3D>();
			IList<Vector3D> nor = new List<Vector3D>();
			IList<Point> tex = new List<Point>();
			int typ = -1;
			if (polydata == null)
				return;
			int commandptr = 0;
			int commandlimit = polydata.Length;
			int[] command = new int[4];
			int cur_vertex, mode, i;
			float[] vtx_state = { 0.0f, 0.0f, 0.0f };
			float[] vtx_trans = { 0.0f, 0.0f, 0.0f };
			cur_vertex = gCurrentVertex; // for vertex_mode
			if (Model.Objects.Count > 0)
			{
				CurrentMatrix = MatrixStack[stackID].Clone();
			}
			else
			{
                CurrentMatrix.LoadIdentity();
			}
			if (Model.Objects.Count > 1)
			{
				//CurrentMatrix.Scale(Model.modelScale, Model.modelScale, Model.modelScale);
			}
			while (commandptr < commandlimit)
			{
				for (i = 0; i < 4; ++i)
				{
					if (commandptr >= commandlimit)
						command[i] = 0xFF;
					else
					{
						command[i] = polydata[commandptr];
						commandptr++;
					}
				}


				for (i = 0; i < 4 && commandptr < commandlimit; i++)
				{
					switch (command[i])
					{
						case 0: // No Operation (for padding packed GXFIFO commands)
							break;
						case 0x10:
							{
								int param = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								switch (param)
								{
									case 0:
										// Gl.glMatrixMode(Gl.GL_PROJECTION_MATRIX);
										break;
									case 1:
										// Gl.glMatrixMode(Gl.GL_MODELVIEW_MATRIX);
										break;
									case 2:
										break;
									case 3:
										// Gl.glMatrixMode(Gl.GL_TEXTURE_MATRIX);
										break;
								}
								break;
							}
						case 0x11: break;
						case 0x12: commandptr += 4; break;
						case 0x13: commandptr += 4; break;
						case 0x14:
							/*
								  MTX_RESTORE - Restore Current Matrix from Stack (W)
								  Sets C=[N]. The stack pointer S is not used, and is left unchanged.
								  Parameter Bit0-4:  Stack Address (0..30) (31 causes overflow in GXSTAT.15)
								  Parameter Bit5-31: Not used
								*/

							stackID = Utils.Read4BytesAsInt32(polydata, commandptr) & 0x1F;// & 0x0000001F;
							commandptr += 4;
							MatrixStack[stackID].CopyValuesTo(CurrentMatrix);
							break;
						case 0x15:
							{
								CurrentMatrix.LoadIdentity();
								break;
							}
						case 0x16:
							{
								for (int j = 0; j < 16; j++)
								{
									CurrentMatrix[j] = (float)Utils.Read4BytesAsInt32(polydata, commandptr) / 4096f;
									commandptr += 4;
								}
								break;
							}
						case 0x17:
							{
								for (int j = 0; j < 4; j++)
								{
									for (int k = 0; k < 3; j++)
									{
										CurrentMatrix[k, j] = (float)Utils.Read4BytesAsInt32(polydata, commandptr) / 4096f;
										commandptr += 4;
									}
								}
								break;
							}
						case 0x18:
							{
								MTX44 f = new MTX44();
								f.LoadIdentity();
								for (int j = 0; j < 16; j++)
								{
									f[j] = (float)Utils.Read4BytesAsInt32(polydata, commandptr) / 4096f;
									commandptr += 4;
								}
								CurrentMatrix.MultMatrix(f).CopyValuesTo(CurrentMatrix);
								break;
							}
						case 0x19:
							{
								MTX44 f = new MTX44();
								f.LoadIdentity();
								for (int j = 0; j < 4; j++)
								{
									for (int k = 0; k < 3; j++)
									{
										f[k, j] = (float)Utils.Read4BytesAsInt32(polydata, commandptr) / 4096f;
										commandptr += 4;
									}
								}
								CurrentMatrix.MultMatrix(f).CopyValuesTo(CurrentMatrix);
								break;
							}
						case 0x1A:
							{
								MTX44 f = new MTX44();
								f.LoadIdentity();
								for (int j = 0; j < 3; j++)
								{
									for (int k = 0; k < 3; j++)
									{
										f[k, j] = (float)Utils.Read4BytesAsInt32(polydata, commandptr) / 4096f;
										commandptr += 4;
									}
								}
								CurrentMatrix.MultMatrix(f).CopyValuesTo(CurrentMatrix);
								break;
							}
						case 0x1b:
							/*
								  MTX_SCALE - Multiply Current Matrix by Scale Matrix (W)
								  Sets C=M*C. Parameters: 3, m[0..2] (MTX_SCALE doesn't change Vector Matrix)
								*/
							{
								int x, y, z;
								x = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								y = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								z = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								//CurrentMatrix[0] = (float)sign(x, 32) / SCALE_IV;
								//CurrentMatrix[5] = (float)sign(y, 32) / SCALE_IV;
								//CurrentMatrix[10] = (float)sign(z, 32) / SCALE_IV;
								//CurrentMatrix.SetValues(scale(CurrentMatrix.Floats, x, y, z));
								CurrentMatrix.Scale((float)x / SCALE_IV / Model.modelScale, (float)y / SCALE_IV / Model.modelScale, (float)z / SCALE_IV / Model.modelScale);
								//CurrentMatrix.Scale((float)sign(x, 32) / SCALE_IV, (float)sign(y, 32) / SCALE_IV, (float)sign(z, 32) / SCALE_IV);
								break;
							}
						case 0x1c:
							/*
								  MTX_TRANS - Mult. Curr. Matrix by Translation Matrix (W)
								  Sets C=M*C. Parameters: 3, m[0..2] (x,y,z position)
								*/
							{
								int x, y, z;
								x = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								y = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								z = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								CurrentMatrix.translate((float)sign(x, 32) / SCALE_IV / Model.modelScale, (float)sign(y, 32) / SCALE_IV / Model.modelScale, (float)sign(z, 32) / SCALE_IV / Model.modelScale);
								break;
							}
						case 0x20: // Directly Set Vertex Color (W)
							{
								Int64 rgb, r, g, b;

								rgb = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								if (gOptColoring)
								{
									r = (rgb >> 0) & 0x1F;
									g = (rgb >> 5) & 0x1F;
									b = (rgb >> 10) & 0x1F;
									if (color)
									{
										Gl.glColor4f(((float)r) / 31.0f, ((float)g) / 31.0f, ((float)b) / 31.0f, m.Alpha / 31.0f);
									}
								}
							}
							break;

						case 0x21:
							/*
								  Set Normal Vector (W)
								  0-9   X-Component of Normal Vector (1bit sign + 9bit fractional part)
								  10-19 Y-Component of Normal Vector (1bit sign + 9bit fractional part)
								  20-29 Z-Component of Normal Vector (1bit sign + 9bit fractional part)
								  30-31 Not used
								*/
							{
								Int64 xyz, x, y, z;

								xyz = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								x = (xyz >> 0) & 0x3FF;
								if ((x & 0x200) != 0)
									x |= -1024;
								y = (xyz >> 10) & 0x3FF;
								if ((y & 0x200) != 0) y |= -1024;
								z = (xyz >> 20) & 0x3FF;
								if ((z & 0x200) != 0) z |= -1024;
								Gl.glNormal3f(((float)x) / 512.0f, ((float)y) / 512.0f, ((float)z) / 512.0f);
								if (writevertex)
								{
									//normals.Add(new float[] { ((float)x) / 512.0f, ((float)y) / 512.0f, ((float)z) / 512.0f });
									//mod.Normals.Add(new System.Windows.Media.Media3D.Vector3D(((float)x) / 512.0f, ((float)y) / 512.0f, ((float)z) / 512.0f))
									nor.Add(new System.Windows.Media.Media3D.Vector3D(((float)x) / 512.0f, ((float)y) / 512.0f, ((float)z) / 512.0f));
								}
								break;
							}
						case 0x22:
							/*
								  Set Texture Coordinates (W)
								  Parameter 1, Bit 0-15   S-Coordinate (X-Coordinate in Texture Source)
								  Parameter 1, Bit 16-31  T-Coordinate (Y-Coordinate in Texture Source)
								  Both values are 1bit sign + 11bit integer + 4bit fractional part.
								  A value of 1.0 (=1 SHL 4) equals to one Texel.
								*/
							{
								Int64 st, s, t;

								st = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								s = (st >> 0) & 0xffff;
								if ((s & 0x8000) != 0) s |= unchecked((int)0xFFFF0000);//-65536;
								t = (st >> 16) & 0xffff;
								if ((t & 0x8000) != 0) t |= unchecked((int)0xFFFF0000);//-65536;
								Gl.glTexCoord2f(((float)s) / 16.0f, ((float)t) / 16.0f);

								if (writevertex)
								{
									//mod.TextureCoordinates.Add(new System.Windows.Point((float)s/1024f,(float)t/1024f));
									tex.Add(new System.Windows.Point(((float)m.scaleS / (float)m.width) * ((float)s / 16f) / (m.flipS + 1), -((float)m.scaleT / (float)m.height) * ((float)t / 16f) / (m.flipT + 1)));
								}
								break;
							}
						case 0x23:
							/*
								  VTX_16 - Set Vertex XYZ Coordinates (W)
								  Parameter 1, Bit 0-15   X-Coordinate (signed, with 12bit fractional part)
								  Parameter 1, Bit 16-31  Y-Coordinate (signed, with 12bit fractional part)
								  Parameter 2, Bit 0-15   Z-Coordinate (signed, with 12bit fractional part)
								  Parameter 2, Bit 16-31  Not used
								*/
							{
								int parameter, x, y, z;

								parameter = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								x = sign((parameter >> 0) & 0xFFFF, 16);
								//if ((x & 0x8000) != 0) x |= unchecked((int)0xFFFF0000);//-65536;
								y = sign((parameter >> 16) & 0xFFFF, 16);
								//if ((y & 0x8000) != 0) y |= unchecked((int)0xFFFF0000);//-65536;

								parameter = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								z = sign(parameter & 0xFFFF, 16);
								// if ((z & 0x8000) != 0) z |= unchecked((int)0xFFFF0000);//-65536;

								vtx_state[0] = ((float)x) / SCALE_IV;
								vtx_state[1] = ((float)y) / SCALE_IV;
								vtx_state[2] = ((float)z) / SCALE_IV;
								if (stackID != -1)
								{
									vtx_trans = CurrentMatrix.MultVector(vtx_state);
									Gl.glVertex3fv(vtx_trans);
									if (writevertex)
									{
										//vertex.Add(vtx_trans);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								else
								{
									Gl.glVertex3fv(vtx_state);
									if (writevertex)
									{
										//vertex.Add(vtx_state);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								if (writevertex)
								{
									//vertex_normal.Add((normals.Count == 0 ? -1 : normals.Count));
								}
								break;
							}
						case 0x24:
							/*
								  VTX_10 - Set Vertex XYZ Coordinates (W)
								  Parameter 1, Bit 0-9    X-Coordinate (signed, with 6bit fractional part)
								  Parameter 1, Bit 10-19  Y-Coordinate (signed, with 6bit fractional part)
								  Parameter 1, Bit 20-29  Z-Coordinate (signed, with 6bit fractional part)
								  Parameter 1, Bit 30-31  Not used
								*/
							{
								int xyz, x, y, z;

								xyz = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								x = sign((xyz >> 0) & 0x3FF, 10);
								// if ((x & 0x200) != 0) x |=  unchecked((int)0xFFFFFC00);//-1024;
								y = sign((xyz >> 10) & 0x3FF, 10);
								//if ((y & 0x200) != 0) y |=  unchecked((int)0xFFFFFC00);//-1024;
								z = sign((xyz >> 20) & 0x3FF, 10);
								// if ((z & 0x200) != 0) z |= unchecked((int)0xFFFFFC00);//-1024;

								vtx_state[0] = (float)x / 64.0f;
								vtx_state[1] = (float)y / 64.0f;
								vtx_state[2] = (float)z / 64.0f;
								if (stackID != -1)
								{
									vtx_trans = CurrentMatrix.MultVector(vtx_state);
									Gl.glVertex3fv(vtx_trans);
									if (writevertex)
									{
										//vertex.Add(vtx_trans);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								else
								{
									Gl.glVertex3fv(vtx_state);
									if (writevertex)
									{
										//vertex.Add(vtx_state);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								if (writevertex)
								{
									//vertex_normal.Add((normals.Count == 0 ? -1 : normals.Count));
								}
								break;
							}
						case 0x25:
							/*
								  VTX_XY - Set Vertex XY Coordinates (W)
								  Parameter 1, Bit 0-15   X-Coordinate (signed, with 12bit fractional part)
								  Parameter 1, Bit 16-31  Y-Coordinate (signed, with 12bit fractional part)
								*/
							{
								int xy, x, y;

								xy = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								x = sign((xy >> 0) & 0xFFFF, 16);
								//if ((x & 0x8000) != 0) x |=  unchecked((int)0xFFFF0000);//-65536;
								y = sign((xy >> 16) & 0xFFFF, 16);
								//if ((y & 0x8000) != 0) y |= unchecked((int)0xFFFF0000);//-65536;

								vtx_state[0] = ((float)x) / SCALE_IV;
								vtx_state[1] = ((float)y) / SCALE_IV;
								if (stackID != -1)
								{
									vtx_trans = CurrentMatrix.MultVector(vtx_state);
									Gl.glVertex3fv(vtx_trans);
									if (writevertex)
									{
										//vertex.Add(vtx_trans);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								else
								{
									Gl.glVertex3fv(vtx_state);
									if (writevertex)
									{
										//vertex.Add(vtx_state);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								if (writevertex)
								{
									//vertex_normal.Add((normals.Count == 0 ? -1 : normals.Count));
								}
								break;
							}
						case 0x26:
							/*
								  VTX_XZ - Set Vertex XZ Coordinates (W)
								  Parameter 1, Bit 0-15   X-Coordinate (signed, with 12bit fractional part)
								  Parameter 1, Bit 16-31  Z-Coordinate (signed, with 12bit fractional part)
								*/
							{
								int xz, x, z;

								xz = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								x = sign((xz >> 0) & 0xFFFF, 16);
								// if ((x & 0x8000) != 0) x |= unchecked((int)0xFFFF0000);//-65536;
								z = sign((xz >> 16) & 0xFFFF, 16);
								// if ((z & 0x8000) != 0) z |= unchecked((int)0xFFFF0000);//-65536;

								vtx_state[0] = ((float)x) / SCALE_IV;
								vtx_state[2] = ((float)z) / SCALE_IV;
								if (stackID != -1)
								{
									vtx_trans = CurrentMatrix.MultVector(vtx_state);
									Gl.glVertex3fv(vtx_trans);
									if (writevertex)
									{
										//vertex.Add(vtx_trans);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								else
								{
									Gl.glVertex3fv(vtx_state);
									if (writevertex)
									{
										//vertex.Add(vtx_state);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								if (writevertex)
								{
									// vertex_normal.Add((normals.Count == 0 ? -1 : normals.Count));
								}
								break;
							}
						case 0x27:
							/*
								  VTX_YZ - Set Vertex YZ Coordinates (W)
								  Parameter 1, Bit 0-15   Y-Coordinate (signed, with 12bit fractional part)
								  Parameter 1, Bit 16-31  Z-Coordinate (signed, with 12bit fractional part)
								*/
							{
								int yz, y, z;
								yz = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								y = sign((yz >> 0) & 0xFFFF, 16);
								//if ((y & 0x8000) != 0) y |= unchecked((int)0xFFFF0000);//-65536;
								z = sign((yz >> 16) & 0xFFFF, 16);
								//if ((z & 0x8000) != 0) z |= unchecked((int)0xFFFF0000);//-65536;

								vtx_state[1] = ((float)y) / SCALE_IV;
								vtx_state[2] = ((float)z) / SCALE_IV;
								if (stackID != -1)
								{
									vtx_trans = CurrentMatrix.MultVector(vtx_state);
									Gl.glVertex3fv(vtx_trans);
									if (writevertex)
									{
										//vertex.Add(vtx_trans);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								else
								{
									Gl.glVertex3fv(vtx_state);
									if (writevertex)
									{
										//vertex.Add(vtx_state);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								if (writevertex)
								{
									//vertex_normal.Add((normals.Count == 0 ? -1 : normals.Count));
								}
								break;
							}
						case 0x28:
							/*
								  VTX_DIFF - Set Relative Vertex Coordinates (W)
								  Parameter 1, Bit 0-9    X-Difference (signed, with 9bit fractional part)
								  Parameter 1, Bit 10-19  Y-Difference (signed, with 9bit fractional part)
								  Parameter 1, Bit 20-29  Z-Difference (signed, with 9bit fractional part)
								  Parameter 1, Bit 30-31  Not used
								*/
							{
								int xyz, x, y, z;
								xyz = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;

								x = sign((xyz >> 0) & 0x3FF, 10);
								//if ((x & 0x200) != 0) x |= unchecked((int)0xFFFFFC00);//-1024;
								y = sign((xyz >> 10) & 0x3FF, 10);
								//if ((y & 0x200) != 0) y |= unchecked((int)0xFFFFFC00);
								z = sign((xyz >> 20) & 0x3FF, 10);
								//if ((z & 0x200) != 0) z |= unchecked((int)0xFFFFFC00);


								vtx_state[0] += ((float)x) / SCALE_IV;
								vtx_state[1] += ((float)y) / SCALE_IV;
								vtx_state[2] += ((float)z) / SCALE_IV;
								if (stackID != -1)
								{
									vtx_trans = CurrentMatrix.MultVector(vtx_state);
									Gl.glVertex3fv(vtx_trans);
									if (writevertex)
									{
										//vertex.Add(vtx_trans);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_trans[0], vtx_trans[1], vtx_trans[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if (nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity, 0));
										}
									}
								}
								else
								{
									Gl.glVertex3fv(vtx_state);
									if (writevertex)
									{
										//vertex.Add(vtx_state);
										//mod.Positions.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										poi.Add(new System.Windows.Media.Media3D.Point3D(vtx_state[0], vtx_state[1], vtx_state[2]));
										if (poi.Count > tex.Count && tex.Count != 0)
										{
											tex.Add(tex[tex.Count - 1]);//new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										else if (tex.Count == 0)
										{
											tex.Add(new System.Windows.Point(double.NegativeInfinity, double.PositiveInfinity));
										}
										if (poi.Count > nor.Count && nor.Count != 0)
										{
											nor.Add(nor[nor.Count - 1]);
										}
										else if(nor.Count == 0)
										{
											nor.Add(new System.Windows.Media.Media3D.Vector3D(double.NegativeInfinity, double.PositiveInfinity,0));
										}
									}
								}
								if (writevertex)
								{
									// vertex_normal.Add((normals.Count == 0 ? -1 : normals.Count));
								}
								break;
							}
						case 0x29:
							{
								int param = Utils.Read4BytesAsInt32(polydata, commandptr);
								commandptr += 4;
								/*int light = (param >> 0) & 0x10;
								int polygonMode = (param >> 4) & 0x4;
								switch (polygonMode)
								{
									case 0:
										mode = Gl.GL_MODULATE;
										break;
									case 1:
										mode = Gl.GL_DECAL;
										break;
									case 2:
										mode = Gl.GL_SHADOW_AMBIENT_SGIX;
										break;
									case 3:
										mode = Gl.GL_QUAD_STRIP;
										break;
									default:
										//return ;// FALSE;
										throw new Exception();
										break;
								}*/
								break;
							}
						case 0x2A: commandptr += 4; break;
						case 0x2B: commandptr += 4; break;

						// lighting commands
						case 0x30: commandptr += 4; break;
						case 0x31: commandptr += 4; break;
						case 0x32: commandptr += 4; break;
						case 0x33:
							{

								commandptr += 4;
								break;
							}
						case 0x34: commandptr += 128; break;
						case 0x40: // Start of Vertex List (W)
							{
								mode = Utils.Read4BytesAsInt32(polydata, commandptr);
								//mode = mode & 0x3;
								commandptr += 4;
								typ = mode;
								switch (mode)
								{
									case 0:
										mode = Gl.GL_TRIANGLES;
										break;
									case 1:
										mode = Gl.GL_QUADS;
										break;
									case 2:
										mode = Gl.GL_TRIANGLE_STRIP;
										break;
									case 3:
										mode = Gl.GL_QUAD_STRIP;
										break;
									default:
										//return ;// FALSE;
										throw new Exception();
								}

								Gl.glBegin(mode);
								break;
							}
						case 0x41:
							Gl.glEnd();

							if (writevertex)
							{
								switch (typ)
								{
									case 0:
										{
											for (int j = 0; j < poi.Count / 3; j++)
											{
												if (tex.Count > j * 3 + 2)
												{
													md[md.Count - 1].AddTriangle(poi[j * 3], poi[j * 3 + 1], poi[j * 3 + 2], tex[j * 3], tex[j * 3 + 1], tex[j * 3 + 2]);
												}
												else
												{
													md[md.Count - 1].AddTriangle(poi[j * 3], poi[j * 3 + 1], poi[j * 3 + 2]);
												}
											}

											break;
										}
									case 1:
										{
											for (int j = 0; j < poi.Count / 4; j++)
											{												
												if (tex.Count > j * 4 + 3)
												{
													md[md.Count - 1].AddQuad(poi[j * 4], poi[j * 4 + 1], poi[j * 4 + 2], poi[j * 4 + 3], tex[j * 4], tex[j * 4 + 1], tex[j * 4 + 2], tex[j * 4 + 3]);
												}
												else
												{
													md[md.Count - 1].AddQuad(poi[j * 4], poi[j * 4 + 1], poi[j * 4 + 2], poi[j * 4 + 3]);
												}
											}
										
											break;
										}
									case 2:
										{
											while (poi.Count > nor.Count)
											{
												nor.Add(nor[nor.Count - 1]);
											}
											while (poi.Count > tex.Count)
											{
												tex.Add(tex[tex.Count - 1]);
											}
											md[md.Count - 1].AddTriangles(poi, nor, tex);
											//md[mo].AddTriangleStrip(poi, nor, tex);
											break;
										}
									case 3:
										{
											while (poi.Count > nor.Count)
											{
												nor.Add(nor[nor.Count - 1]);
											}
											while (poi.Count > tex.Count)
											{
												tex.Add(tex[tex.Count - 1]);
											}
											md[md.Count - 1].AddQuads(poi, nor, tex);
											//md.AddQuads(poi, nor, tex);
											break;
										}
									default:
										//return ;// FALSE;
										break;
								}
							}
							nor.Clear();
							tex.Clear();
							poi.Clear();
							//nr++;
							// for vertex mode, display at maximum certain number of vertex-list
							// decrease cur_vertex so that when we reach 0, stop rendering any further
							cur_vertex--;
							if (cur_vertex < 0 && gOptVertexMode)
								return; //TRUE;
							break;
						case 0x50: commandptr += 4; break;
						case 0x60: commandptr += 4; break;
						case 0x70: commandptr += 12; break;
						case 0x71: commandptr += 8; break;
						case 0x72: commandptr += 4; break;
						default:
							break;
						//return FALSE;
					}
				}
			}
		}
		public static int sign(int data, int size)
		{
			if ((data & 1 << size - 1) != 0)
				data |= -1 << size;
			return data;
		}

		/// <summary>
		/// Process polygon 3d commands.
		/// </summary>
		/// <param name="polydata">Data of specific polygon.</param>
        public void ClearOBJ()
        {
            md.Clear();
            writevertex = true;
        }
        public void RipModel(string file)
        {
            NSBMDPolygon polygon;
            int matId;
            NSBMDMaterial material;
            int num3;
            DiffuseMaterial material2;
            MeshBuilder builder;
            Model3D modeld;
            MTX44 mtx = new MTX44();
            List<Group> list = new List<Group>();
            List<Vector3> list2 = new List<Vector3>();
            for (int i = 0; i < (this.Model.Polygons.Count - 1); i++)
            {
                polygon = this.Model.Polygons[i];
                matId = polygon.MatId;
                material = this.Model.Materials[matId];
            }
            MTX44 mtx2 = new MTX44();
            mtx2.LoadIdentity();
            for (num3 = 0; num3 < this.Model.Objects.Count; num3++)
            {
                NSBMDObject obj2 = this.Model.Objects[num3];
                float[] transVect = obj2.TransVect;
                float[] numArray2 = loadIdentity();
                if (obj2.RestoreID != -1)
                {
                    mtx2 = MatrixStack[obj2.RestoreID];
                }
                if (obj2.StackID != -1)
                {
                    if (obj2.visible)
                    {
                        MTX44 b = new MTX44();
                        b.SetValues(obj2.materix);
                        mtx2 = mtx2.MultMatrix(b);
                    }
                    else
                    {
                        mtx2.Zero();
                    }
                    MatrixStack[obj2.StackID] = mtx2;
                    stackID = obj2.StackID;
                    float[] numArray3 = mtx2.MultVector(new float[3]);
                    list2.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
                }
            }
            num3 = 0;
            while (num3 < (this.Model.Polygons.Count - 1))
            {
                polygon = this.Model.Polygons[num3];
                if ((gOptTexture && !gOptWireFrame) && g_mat)
                {
                    matId = polygon.MatId;
                    if (matId == -1)
                    {
                        this.mattt.Add(new ImageBrush());
                    }
                    else
                    {
                        this.mattt.Add(this.matt[matId]);
                        material = this.Model.Materials[matId];
                    }
                }
                stackID = polygon.StackID;
                list.Add(Process3DCommandRipper(polygon.PolyData, Model.Materials[polygon.MatId], polygon.JointID, true));
                num3++;
            }
            File.Create(file).Close();
            ObjExporter exporter = new ObjExporter(file, "Created with DS Pokémon Rom Editor " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            int index = 0;
            foreach (Group group in list)
            {
                ImageBrush brush = new ImageBrush();
                try
                {
                    brush.ImageSource = this.mattt[index].ImageSource;
                    brush.Opacity = this.mattt[index].Opacity;
                    brush.Viewbox = this.mattt[index].Viewbox;
                    brush.ViewboxUnits = this.mattt[index].ViewboxUnits;
                    brush.Viewport = this.mattt[index].Viewport;
                    brush.ViewportUnits = this.mattt[index].ViewportUnits;
                }
                catch
                {
                }
                if (brush.ImageSource != null)
                {
                    material2 = new DiffuseMaterial(brush);
                }
                else
                {
                    material2 = new DiffuseMaterial(new SolidColorBrush());
                }
                material2.SetValue(matName, this.Model.Materials[this.Model.Polygons[index].MatId].MaterialName);
                material2.SetValue(polyName, this.Model.Polygons[index].Name);
                material2.Brush.Opacity = ((float)this.Model.Materials[this.Model.Polygons[index].MatId].Alpha) / 31f;
                material2.AmbientColor = Color.FromArgb(this.Model.Materials[this.Model.Polygons[index].MatId].AmbientColor.A, this.Model.Materials[this.Model.Polygons[index].MatId].AmbientColor.R, this.Model.Materials[this.Model.Polygons[index].MatId].AmbientColor.G, this.Model.Materials[this.Model.Polygons[index].MatId].AmbientColor.B);
                material2.Color = Color.FromArgb(0xff, this.Model.Materials[this.Model.Polygons[index].MatId].DiffuseColor.R, this.Model.Materials[this.Model.Polygons[index].MatId].DiffuseColor.G, this.Model.Materials[this.Model.Polygons[index].MatId].DiffuseColor.B);
                builder = new MeshBuilder();
                foreach (MKDS_Course_Editor.Export3DTools.Polygon polygon2 in group)
                {
                    IList<Point3D> list3;
                    IList<Vector3D> list4;
                    IList<System.Windows.Point> list5;
                    switch (polygon2.PolyType)
                    {
                        case PolygonType.Triangle:
                            list3 = new List<Point3D>();
                            list4 = new List<Vector3D>();
                            list5 = new List<Point>();
                            num3 = 0;
                            goto Label_08AB;

                        case PolygonType.Quad:
                            list3 = new List<Point3D>();
                            list4 = new List<Vector3D>();
                            list5 = new List<Point>();
                            num3 = 0;
                            goto Label_0C5E;

                        case PolygonType.TriangleStrip:
                            list3 = new List<Point3D>();
                            list4 = new List<Vector3D>();
                            list5 = new List<Point>();
                            num3 = 0;
                            goto Label_0D66;

                        case PolygonType.QuadStrip:
                            list3 = new List<Point3D>();
                            list4 = new List<Vector3D>();
                            list5 = new List<Point>();
                            num3 = 0;
                            goto Label_0E7C;

                        default:
                            {
                                continue;
                            }
                    }
                Label_0608:
                    list3.Add(new Point3D((double)polygon2.Vertex[num3].X, (double)polygon2.Vertex[num3].Y, (double)polygon2.Vertex[num3].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3].X, (double)polygon2.Normals[num3].Y, (double)polygon2.Normals[num3].Z));
                    list5.Add(new System.Windows.Point((double)polygon2.TexCoords[num3].X, (double)polygon2.TexCoords[num3].Y));
                    list3.Add(new Point3D((double)polygon2.Vertex[num3 + 1].X, (double)polygon2.Vertex[num3 + 1].Y, (double)polygon2.Vertex[num3 + 1].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3 + 1].X, (double)polygon2.Normals[num3 + 1].Y, (double)polygon2.Normals[num3 + 1].Z));
                    list5.Add(new Point((double)polygon2.TexCoords[num3 + 1].X, (double)polygon2.TexCoords[num3 + 1].Y));
                    list3.Add(new Point3D((double)polygon2.Vertex[num3 + 2].X, (double)polygon2.Vertex[num3 + 2].Y, (double)polygon2.Vertex[num3 + 2].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3 + 2].X, (double)polygon2.Normals[num3 + 2].Y, (double)polygon2.Normals[num3 + 2].Z));
                    list5.Add(new Point((double)polygon2.TexCoords[num3 + 2].X, (double)polygon2.TexCoords[num3 + 2].Y));
                    builder.AddTriangles(list3, list4, list5);
                    list3.Clear();
                    list4.Clear();
                    list5.Clear();
                    num3 += 3;
                Label_08AB:
                    if (num3 < polygon2.Vertex.Length)
                    {
                        goto Label_0608;
                    }
                    continue;
                Label_08E4:
                    list3.Add(new Point3D((double)polygon2.Vertex[num3].X, (double)polygon2.Vertex[num3].Y, (double)polygon2.Vertex[num3].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3].X, (double)polygon2.Normals[num3].Y, (double)polygon2.Normals[num3].Z));
                    list5.Add(new System.Windows.Point((double)polygon2.TexCoords[num3].X, (double)polygon2.TexCoords[num3].Y));
                    list3.Add(new Point3D((double)polygon2.Vertex[num3 + 1].X, (double)polygon2.Vertex[num3 + 1].Y, (double)polygon2.Vertex[num3 + 1].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3 + 1].X, (double)polygon2.Normals[num3 + 1].Y, (double)polygon2.Normals[num3 + 1].Z));
                    list5.Add(new System.Windows.Point((double)polygon2.TexCoords[num3 + 1].X, (double)polygon2.TexCoords[num3 + 1].Y));
                    list3.Add(new Point3D((double)polygon2.Vertex[num3 + 2].X, (double)polygon2.Vertex[num3 + 2].Y, (double)polygon2.Vertex[num3 + 2].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3 + 2].X, (double)polygon2.Normals[num3 + 2].Y, (double)polygon2.Normals[num3 + 2].Z));
                    list5.Add(new System.Windows.Point((double)polygon2.TexCoords[num3 + 2].X, (double)polygon2.TexCoords[num3 + 2].Y));
                    list3.Add(new Point3D((double)polygon2.Vertex[num3 + 3].X, (double)polygon2.Vertex[num3 + 3].Y, (double)polygon2.Vertex[num3 + 3].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3 + 3].X, (double)polygon2.Normals[num3 + 3].Y, (double)polygon2.Normals[num3 + 3].Z));
                    list5.Add(new System.Windows.Point((double)polygon2.TexCoords[num3 + 3].X, (double)polygon2.TexCoords[num3 + 3].Y));
                    builder.AddQuads(list3, list4, list5);
                    list3.Clear();
                    list4.Clear();
                    list5.Clear();
                    num3 += 4;
                Label_0C5E:
                    if (num3 < polygon2.Vertex.Length)
                    {
                        goto Label_08E4;
                    }
                    continue;
                Label_0C97:
                    list3.Add(new Point3D((double)polygon2.Vertex[num3].X, (double)polygon2.Vertex[num3].Y, (double)polygon2.Vertex[num3].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3].X, (double)polygon2.Normals[num3].Y, (double)polygon2.Normals[num3].Z));
                    list5.Add(new Point((double)polygon2.TexCoords[num3].X, (double)polygon2.TexCoords[num3].Y));
                    num3++;
                Label_0D66:
                    if (num3 < polygon2.Vertex.Length)
                    {
                        goto Label_0C97;
                    }
                    builder.AddTriangleStrip(list3, list4, list5);
                    continue;
                Label_0DAD:
                    list3.Add(new Point3D((double)polygon2.Vertex[num3].X, (double)polygon2.Vertex[num3].Y, (double)polygon2.Vertex[num3].Z));
                    list4.Add(new Vector3D((double)polygon2.Normals[num3].X, (double)polygon2.Normals[num3].Y, (double)polygon2.Normals[num3].Z));
                    list5.Add(new System.Windows.Point((double)polygon2.TexCoords[num3].X, (double)polygon2.TexCoords[num3].Y));
                    num3++;
                Label_0E7C:
                    if (num3 < polygon2.Vertex.Length)
                    {
                        goto Label_0DAD;
                    }
                    builder.AddTriangleStrip(list3, list4, list5);
                }
                modeld = new GeometryModel3D(builder.ToMesh(false), material2);
                exporter.Export(modeld);
                index++;
            }
            
            exporter.Close();
            this.writevertex = false;
        }
        private Group Process3DCommandRipper(byte[] polydata, NSBMDMaterial m, int jointID, bool color)
        {
            Group group = new Group();
            Vector3 item = new Vector3(float.NaN, 0f, 0f);
            System.Drawing.Color white = System.Drawing.Color.White;
            Vector2 vector2 = new Vector2(float.NaN, 0f);
            List<Vector3> list = new List<Vector3>();
            List<Vector3> list2 = new List<Vector3>();
            List<Vector2> list3 = new List<Vector2>();
            int num = -1;
            if (polydata != null)
            {
                int index = 0;
                int length = polydata.Length;
                int[] numArray = new int[4];
                float[] numArray4 = new float[3];
                float[] v = numArray4;
                numArray4 = new float[3];
                float[] numArray3 = numArray4;
                int gCurrentVertex = NSBMDGlRenderer.gCurrentVertex;
                if (this.Model.Objects.Count > 0)
                {
                    CurrentMatrix = MatrixStack[stackID].Clone();
                }
                else
                {
                    CurrentMatrix.LoadIdentity();
                }
                if (this.Model.Objects.Count > 1)
                {
                }
                while (index < length)
                {
                    int num7 = 0;
                    while (num7 < 4)
                    {
                        if (index >= length)
                        {
                            numArray[num7] = 0xff;
                        }
                        else
                        {
                            numArray[num7] = polydata[index];
                            index++;
                        }
                        num7++;
                    }
                    for (num7 = 0; (num7 < 4) && (index < length); num7++)
                    {
                        int num6;
                        int num8;
                        int num9;
                        int num10;
                        MTX44 mtx;
                        int num11;
                        int num12;
                        int num13;
                        int num26;
                        switch (numArray[num7])
                        {
                            case 0x70:
                                {
                                    index += 12;
                                    continue;
                                }
                            case 0x71:
                                {
                                    index += 8;
                                    continue;
                                }
                            case 0x72:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 0x60:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 80:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                            case 0x1d:
                            case 30:
                            case 0x1f:
                            case 0x2c:
                            case 0x2d:
                            case 0x2e:
                            case 0x2f:
                            case 0x11:
                                {
                                    continue;
                                }
                            case 0x10:
                                {
                                    num8 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    continue;
                                }
                            case 0x12:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 0x13:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 20:
                                {
                                    stackID = Utils.Read4BytesAsInt32(polydata, index) & 0x1f;
                                    index += 4;
                                    MatrixStack[stackID].CopyValuesTo(CurrentMatrix);
                                    continue;
                                }
                            case 0x15:
                                {
                                    CurrentMatrix.LoadIdentity();
                                    continue;
                                }
                            case 0x16:
                                {
                                    num9 = 0;
                                    while (num9 < 0x10)
                                    {
                                        CurrentMatrix[num9] = ((float)Utils.Read4BytesAsInt32(polydata, index)) / 4096f;
                                        index += 4;
                                        num9++;
                                    }
                                    continue;
                                }
                            case 0x17:
                                num9 = 0;
                                goto Label_039C;

                            case 0x18:
                                mtx = new MTX44();
                                mtx.LoadIdentity();
                                num9 = 0;
                                goto Label_03E8;

                            case 0x19:
                                mtx = new MTX44();
                                mtx.LoadIdentity();
                                num9 = 0;
                                goto Label_0466;

                            case 0x1a:
                                mtx = new MTX44();
                                mtx.LoadIdentity();
                                num9 = 0;
                                goto Label_04E3;

                            case 0x1b:
                                {
                                    num11 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num12 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num13 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    CurrentMatrix.Scale((((float)num11) / 4096f) / this.Model.modelScale, (((float)num12) / 4096f) / this.Model.modelScale, (((float)num13) / 4096f) / this.Model.modelScale);
                                    continue;
                                }
                            case 0x1c:
                                {
                                    num11 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num12 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num13 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    CurrentMatrix.translate((((float)sign(num11, 0x20)) / 4096f) / this.Model.modelScale, (((float)sign(num12, 0x20)) / 4096f) / this.Model.modelScale, (((float)sign(num13, 0x20)) / 4096f) / this.Model.modelScale);
                                    continue;
                                }
                            case 0x20:
                                {
                                    long num14 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    if (gOptColoring)
                                    {
                                        long num15 = num14 & 0x1fL;
                                        long num16 = (num14 >> 5) & 0x1fL;
                                        long num17 = (num14 >> 10) & 0x1fL;
                                        if (color)
                                        {
                                            white = System.Drawing.Color.FromArgb((int)(((float)m.Alpha) / 31f), (int)(((float)num15) / 31f), (int)(((float)num16) / 31f), (int)(((float)num17) / 31f));
                                        }
                                    }
                                    continue;
                                }
                            case 0x21:
                                {
                                    long num18 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    long num19 = num18 & 0x3ffL;
                                    if ((num19 & 0x200L) != 0L)
                                    {
                                        num19 |= -1024L;
                                    }
                                    long num20 = (num18 >> 10) & 0x3ffL;
                                    if ((num20 & 0x200L) != 0L)
                                    {
                                        num20 |= -1024L;
                                    }
                                    long num21 = (num18 >> 20) & 0x3ffL;
                                    if ((num21 & 0x200L) != 0L)
                                    {
                                        num21 |= -1024L;
                                    }
                                    item = new Vector3(((float)num19) / 512f, ((float)num20) / 512f, ((float)num21) / 512f);
                                    continue;
                                }
                            case 0x22:
                                {
                                    long num22 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    long num23 = num22 & 0xffffL;
                                    if ((num23 & 0x8000L) != 0L)
                                    {
                                        num23 |= -65536L;
                                    }
                                    long num24 = (num22 >> 0x10) & 0xffffL;
                                    if ((num24 & 0x8000L) != 0L)
                                    {
                                        num24 |= -65536L;
                                    }
                                    vector2 = new Vector2(((m.scaleS / ((float)m.width)) * (((float)num23) / 16f)) / ((float)(m.flipS + 1)), (-(m.scaleT / ((float)m.height)) * (((float)num24) / 16f)) / ((float)(m.flipT + 1)));
                                    continue;
                                }
                            case 0x23:
                                {
                                    int num25 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num11 = sign(num25 & 0xffff, 0x10);
                                    num12 = sign((num25 >> 0x10) & 0xffff, 0x10);
                                    num25 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num13 = sign(num25 & 0xffff, 0x10);
                                    v[0] = ((float)num11) / 4096f;
                                    v[1] = ((float)num12) / 4096f;
                                    v[2] = ((float)num13) / 4096f;
                                    if (stackID == -1)
                                    {
                                        goto Label_08E2;
                                    }
                                    numArray3 = CurrentMatrix.MultVector(v);
                                    list.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
                                    list2.Add(item);
                                    list3.Add(vector2);
                                    goto Label_090F;
                                }
                            case 0x24:
                                num26 = Utils.Read4BytesAsInt32(polydata, index);
                                index += 4;
                                num11 = sign(num26 & 0x3ff, 10);
                                num12 = sign((num26 >> 10) & 0x3ff, 10);
                                num13 = sign((num26 >> 20) & 0x3ff, 10);
                                v[0] = ((float)num11) / 64f;
                                v[1] = ((float)num12) / 64f;
                                v[2] = ((float)num13) / 64f;
                                if (stackID == -1)
                                {
                                    goto Label_09E1;
                                }
                                numArray3 = CurrentMatrix.MultVector(v);
                                list.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
                                list2.Add(item);
                                list3.Add(vector2);
                                goto Label_0A0E;

                            case 0x25:
                                {
                                    int num27 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num11 = sign(num27 & 0xffff, 0x10);
                                    num12 = sign((num27 >> 0x10) & 0xffff, 0x10);
                                    v[0] = ((float)num11) / 4096f;
                                    v[1] = ((float)num12) / 4096f;
                                    if (stackID == -1)
                                    {
                                        goto Label_0ABF;
                                    }
                                    numArray3 = CurrentMatrix.MultVector(v);
                                    list.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
                                    list2.Add(item);
                                    list3.Add(vector2);
                                    goto Label_0AEC;
                                }
                            case 0x26:
                                {
                                    int num28 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num11 = sign(num28 & 0xffff, 0x10);
                                    num13 = sign((num28 >> 0x10) & 0xffff, 0x10);
                                    v[0] = ((float)num11) / 4096f;
                                    v[2] = ((float)num13) / 4096f;
                                    if (stackID == -1)
                                    {
                                        goto Label_0B9D;
                                    }
                                    numArray3 = CurrentMatrix.MultVector(v);
                                    list.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
                                    list2.Add(item);
                                    list3.Add(vector2);
                                    goto Label_0BCA;
                                }
                            case 0x27:
                                {
                                    int num29 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    num12 = sign(num29 & 0xffff, 0x10);
                                    num13 = sign((num29 >> 0x10) & 0xffff, 0x10);
                                    v[1] = ((float)num12) / 4096f;
                                    v[2] = ((float)num13) / 4096f;
                                    if (stackID == -1)
                                    {
                                        goto Label_0C7B;
                                    }
                                    numArray3 = CurrentMatrix.MultVector(v);
                                    list.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
                                    list2.Add(item);
                                    list3.Add(vector2);
                                    goto Label_0CA8;
                                }
                            case 40:
                                num26 = Utils.Read4BytesAsInt32(polydata, index);
                                index += 4;
                                num11 = sign(num26 & 0x3ff, 10);
                                num12 = sign((num26 >> 10) & 0x3ff, 10);
                                num13 = sign((num26 >> 20) & 0x3ff, 10);
                                v[0] += ((float)num11) / 4096f;
                                v[1] += ((float)num12) / 4096f;
                                v[2] += ((float)num13) / 4096f;
                                if (stackID == -1)
                                {
                                    goto Label_0DAA;
                                }
                                numArray3 = CurrentMatrix.MultVector(v);
                                list.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
                                list2.Add(item);
                                list3.Add(vector2);
                                goto Label_0DD7;

                            case 0x29:
                                {
                                    num8 = Utils.Read4BytesAsInt32(polydata, index);
                                    index += 4;
                                    continue;
                                }
                            case 0x2a:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 0x2b:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 0x30:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 0x31:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 50:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 0x33:
                                {
                                    index += 4;
                                    continue;
                                }
                            case 0x34:
                                {
                                    index += 0x80;
                                    continue;
                                }
                            case 0x40:
                                num6 = Utils.Read4BytesAsInt32(polydata, index);
                                index += 4;
                                num = num6;
                                switch (num6)
                                {
                                    case 0:
                                        goto Label_0E92;

                                    case 1:
                                        goto Label_0E97;

                                    case 2:
                                        goto Label_0E9C;

                                    case 3:
                                        goto Label_0EA1;
                                }
                                throw new Exception();

                            case 0x41:
                                switch (num)
                                {
                                    case 0:
                                        goto Label_0ED1;

                                    case 1:
                                        goto Label_0EF6;

                                    case 2:
                                        goto Label_0F1B;

                                    case 3:
                                        goto Label_0F40;
                                }
                                goto Label_0F67;

                            default:
                                {
                                    continue;
                                }
                        }
                    Label_0358:
                        num10 = 0;
                        while (num10 < 3)
                        {
                            CurrentMatrix[num10, num9] = ((float)Utils.Read4BytesAsInt32(polydata, index)) / 4096f;
                            index += 4;
                            num9++;
                        }
                        num9++;
                    Label_039C:
                        if (num9 < 4)
                        {
                            goto Label_0358;
                        }
                        continue;
                    Label_03C1:
                        mtx[num9] = ((float)Utils.Read4BytesAsInt32(polydata, index)) / 4096f;
                        index += 4;
                        num9++;
                    Label_03E8:
                        if (num9 < 0x10)
                        {
                            goto Label_03C1;
                        }
                        CurrentMatrix.MultMatrix(mtx).CopyValuesTo(CurrentMatrix);
                        continue;
                    Label_0425:
                        num10 = 0;
                        while (num10 < 3)
                        {
                            mtx[num10, num9] = ((float)Utils.Read4BytesAsInt32(polydata, index)) / 4096f;
                            index += 4;
                            num9++;
                        }
                        num9++;
                    Label_0466:
                        if (num9 < 4)
                        {
                            goto Label_0425;
                        }
                        CurrentMatrix.MultMatrix(mtx).CopyValuesTo(CurrentMatrix);
                        continue;
                    Label_04A2:
                        num10 = 0;
                        while (num10 < 3)
                        {
                            mtx[num10, num9] = ((float)Utils.Read4BytesAsInt32(polydata, index)) / 4096f;
                            index += 4;
                            num9++;
                        }
                        num9++;
                    Label_04E3:
                        if (num9 < 3)
                        {
                            goto Label_04A2;
                        }
                        CurrentMatrix.MultMatrix(mtx).CopyValuesTo(CurrentMatrix);
                        continue;
                    Label_08E2:
                        list.Add(new Vector3(v[0], v[1], v[2]));
                        list2.Add(item);
                        list3.Add(vector2);
                    Label_090F:
                        if (this.writevertex)
                        {
                        }
                        continue;
                    Label_09E1:
                        list.Add(new Vector3(v[0], v[1], v[2]));
                        list2.Add(item);
                        list3.Add(vector2);
                    Label_0A0E:
                        if (this.writevertex)
                        {
                        }
                        continue;
                    Label_0ABF:
                        list.Add(new Vector3(v[0], v[1], v[2]));
                        list2.Add(item);
                        list3.Add(vector2);
                    Label_0AEC:
                        if (this.writevertex)
                        {
                        }
                        continue;
                    Label_0B9D:
                        list.Add(new Vector3(v[0], v[1], v[2]));
                        list2.Add(item);
                        list3.Add(vector2);
                    Label_0BCA:
                        if (this.writevertex)
                        {
                        }
                        continue;
                    Label_0C7B:
                        list.Add(new Vector3(v[0], v[1], v[2]));
                        list2.Add(item);
                        list3.Add(vector2);
                    Label_0CA8:
                        if (this.writevertex)
                        {
                        }
                        continue;
                    Label_0DAA:
                        list.Add(new Vector3(v[0], v[1], v[2]));
                        list2.Add(item);
                        list3.Add(vector2);
                    Label_0DD7:
                        if (this.writevertex)
                        {
                        }
                        continue;
                    Label_0E92:
                        num6 = 4;
                        continue;
                    Label_0E97:
                        num6 = 7;
                        continue;
                    Label_0E9C:
                        num6 = 5;
                        continue;
                    Label_0EA1:
                        num6 = 8;
                        continue;
                    Label_0ED1:
                        group.Add(new MKDS_Course_Editor.Export3DTools.Polygon(PolygonType.Triangle, list2.ToArray(), list3.ToArray(), list.ToArray()));
                        goto Label_0F67;
                    Label_0EF6:
                        group.Add(new MKDS_Course_Editor.Export3DTools.Polygon(PolygonType.Quad, list2.ToArray(), list3.ToArray(), list.ToArray()));
                        goto Label_0F67;
                    Label_0F1B:
                        group.Add(new MKDS_Course_Editor.Export3DTools.Polygon(PolygonType.TriangleStrip, list2.ToArray(), list3.ToArray(), list.ToArray()));
                        goto Label_0F67;
                    Label_0F40:
                        group.Add(new MKDS_Course_Editor.Export3DTools.Polygon(PolygonType.QuadStrip, list2.ToArray(), list3.ToArray(), list.ToArray()));
                    Label_0F67:
                        list2.Clear();
                        list.Clear();
                        list3.Clear();
                        gCurrentVertex--;
                        if ((gCurrentVertex < 0) && gOptVertexMode)
                        {
                            return group;
                        }
                    }
                }
            }
            return group;
        }

		#endregion Methods

		/*------------------------------------------------------------
	combine texture + palette data obtained from NSBMD / NSBTX files
	This functions convert all textures of a model into 32-bit bitmap for OpenGL use
	A model has a number of "materials"; Material means a pair of texture and palette.
------------------------------------------------------------*/
	}
}