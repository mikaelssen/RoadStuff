using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Raylib;
using raylib = Raylib.Raylib;
namespace RoadGenerator2000
{
	class Program
	{

		static Camera3D camera;
		static List<Node> RoadNodes;
		static Random r = new Random();
		static Texture2D heightmap;
		static Image Heightmapimage;
		static Model mapmodel;
		static Vector3 mappos;
		static Ray ray;
		static RayHitInfo hit;

		static void Main(string[] args)
		{
			int ScreenWidth = 320 * 3;
			int ScreenHeight = 240 * 4;
			
			
			raylib.InitWindow(ScreenWidth, ScreenHeight, "Roadmaker2000");
			Heightmapimage = raylib.GenImagePerlinNoise(600, 600, 0, 0, 20);
			heightmap = raylib.LoadTextureFromImage(Heightmapimage);
			mapmodel = raylib.LoadModelFromMesh(raylib.GenMeshHeightmap(Heightmapimage, new Vector3(600, 30, 600)));
			mapmodel.material.maps[0].texture = heightmap;
			mappos = new Vector3(0, 0, 0);

			RoadNodes = new List<Node>();


			camera = new Camera3D (new Vector3( 18.0f, 16.0f, 18.0f), new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3(0.0f, 1.0f, 0.0f ), 45f,CameraType.CAMERA_PERSPECTIVE);
			raylib.SetCameraMode( camera, (int)CameraMode.CAMERA_FREE);

			Generate();
			raylib.SetTargetFPS(60);

			while (!raylib.WindowShouldClose())
			{
				Update();
				Draw();

			}
			Console.ReadLine();
		}

		public static double Distance(Vector3 v1, Vector3 v2)
		{
			float dx = v1.x - v2.x;
			float dy = v1.y - v2.y;
			float dz = v1.z - v2.z;
			return Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
		}

		public static void GetClosest(Node v1, List<Node> nodes)
		{
			Node closest = null;
			double closestdistance = 1000000;

			foreach (var v2 in RoadNodes)
			{
				if (v1 == v2 || v1.Neigbors.Contains(v2) || v2.Neigbors.Contains(v1) || v2.Neigbors.Count > 4)
					continue;
				if (Distance(v1.position, v2.position) < closestdistance)
				{
					closestdistance = Distance(v1.position, v2.position);
					closest = v2;
				}
			}
			if (closest != null)
			{
				v1.Neigbors.Add(closest);
				closest.Neigbors.Add(v1);
			}
		}


		
		static unsafe void Generate()
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			RoadNodes.Clear();

			
			for (int i = 0; i < 10; i++) //ray checking is slow as fuck, solve this
			{
				RoadNodes.Add(new Node(new Vector3(r.Next(0, 600), 40, r.Next(0, 600))));

				int count = mapmodel.mesh.vertexCount / 3;
				Vector3* vertdata = (Vector3*)mapmodel.mesh.vertices;
				
				Vector3 y = vertdata[(int)(RoadNodes[i].position.x + RoadNodes[i].position.z) * 3 + 1];
				y = raylib.Vector3Transform(y, mapmodel.transform);

				Console.WriteLine(y);
				RoadNodes[i].position.y = y.y;

			}
			foreach (var v1 in RoadNodes)
			{
				while (v1.Neigbors.Count <= 3)
					GetClosest(v1, RoadNodes);
				while (v1.Neigbors.Count > 2)
					v1.Neigbors.RemoveAt(2);
			}
			stopWatch.Stop();
			Console.WriteLine("Generation supposedly took: " + stopWatch.Elapsed);
		}
			
		static void Update()
		{
			raylib.UpdateCamera(ref camera);

			if (raylib.IsKeyDown((int)Key.KEY_Z)) camera.target = new Vector3(0.0f, 0.0f, 0.0f);

			if (raylib.IsKeyPressed((int)Key.KEY_SPACE)) Generate();

		}

		static void Draw()
		{

			raylib.ClearBackground(raylib.RAYWHITE);
			raylib.BeginDrawing();

			raylib.BeginMode3D(camera);

			
			raylib.DrawModel(mapmodel, mappos, 1, Color.WHITE);

			foreach (var node in RoadNodes)
			{
				
				raylib.DrawCube(node.position, 1, 1, 1, Color.BLUE);
				foreach (var links in node.Neigbors)
				{
					raylib.DrawLine3D(node.position, links.position, Color.DARKPURPLE);
				}
			}

			//raylib.DrawGrid(20, 5);

			raylib.EndMode3D();

			//raylib.DrawTexture(heightmap, 10, 20, Color.WHITE);

			raylib.EndDrawing();
		}
	}
}
