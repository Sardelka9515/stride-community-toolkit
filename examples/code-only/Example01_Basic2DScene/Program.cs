using Example01_Basic2DScene;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.ProceduralModels;
using Stride.CommunityToolkit.Rendering.Compositing;
using Stride.CommunityToolkit.Rendering.Utilities;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stride.Input;
using Stride.Physics;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using System.Reflection;

using var game = new Game();

var boxSize = new Vector3(0.2f);
var model = new Model();
int cubes = 0;

game.Run(start: Start, update: Update);


void Start(Scene rootScene)
{
    game.Window.AllowUserResizing = true;
    game.Window.Title = "2D Example";

    //game.SetupBase3DScene();

    game.AddGraphicsCompositor().AddCleanUIStage();

    game.Add3DCamera().AddInteractiveCameraScript();
    //game.Add2DCamera().AddInteractiveCameraScript();

    game.AddDirectionalLight();
    game.AddSkybox();

    // Make sure you also update 2D Groud collider if you are testing this
    //game.Add2DGround();
    game.Add3DGround();

    game.AddGroundGizmo(new(-2, 1, 0), showAxisName: true);
    game.AddProfiler();

    //AddSpriteBatchRenderer(rootScene);

    var planeMesh = GiveMeAPlane();

    model = new Model
    {
        new Mesh {
            Draw = planeMesh.ToMeshDraw(game.GraphicsDevice),
            MaterialIndex = 0
        }
    };

    //var simulation = game.SceneSystem.SceneInstance.GetProcessor<PhysicsProcessor>()?.Simulation;

    //simulation.FixedTimeStep = 1f / 120;

    Add2DBoxes(rootScene, 1);

    AddBackground(rootScene);

    //Add3DBoxes(rootScene);
}

void Update(Scene scene, GameTime time)
{
    if (!game.Input.HasKeyboard) return;

    if (game.Input.IsKeyPressed(Keys.N))
    {
        Add3DBoxes(scene, 10);

        cubes = scene.Entities.Where(w => w.Name == "Cube").Count();
    }

    if (game.Input.IsKeyPressed(Keys.M))
    {
        Add2DBoxes(scene, 10);

        cubes = scene.Entities.Where(w => w.Name == "Cube").Count();
    }

    game.DebugTextSystem.Print($"N - generate 3D cubes", new Int2(x: 20, y: 30));
    game.DebugTextSystem.Print($"M - generate 2D squares", new Int2(x: 20, y: 50));
    game.DebugTextSystem.Print($"Cubes: {cubes}", new Int2(x: 20, y: 70));
}

MeshBuilder GiveMeAPlane()
{
    var meshBuilder = new MeshBuilder();

    meshBuilder.WithIndexType(IndexingType.Int16);
    meshBuilder.WithPrimitiveType(PrimitiveType.TriangleList);

    var position = meshBuilder.WithPosition<Vector2>();
    //var color = meshBuilder.WithColor<Color>();

    meshBuilder.AddVertex();
    meshBuilder.SetElement(position, new Vector2(0, 0));
    //meshBuilder.SetElement(color, Color.Red);

    meshBuilder.AddVertex();
    meshBuilder.SetElement(position, new Vector2(0, boxSize.X));
    //meshBuilder.SetElement(color, Color.Green);

    meshBuilder.AddVertex();
    meshBuilder.SetElement(position, new Vector2(boxSize.X, boxSize.X));
    //meshBuilder.SetElement(color, Color.Blue);

    meshBuilder.AddVertex();
    meshBuilder.SetElement(position, new Vector2(boxSize.X, 0));
    //meshBuilder.SetElement(color, Color.Yellow);

    meshBuilder.AddIndex(0);
    meshBuilder.AddIndex(1);
    meshBuilder.AddIndex(2);

    meshBuilder.AddIndex(0);
    meshBuilder.AddIndex(2);
    meshBuilder.AddIndex(3);

    return meshBuilder;
}

// Image by brgfx, Free license, Attribution is required: https://www.freepik.com/free-vector/nature-roadside-background-scene_40169781.htm#query=2d%20game%20background&position=13&from_view=keyword&track=ais&uuid=dde78bdc-b045-4f91-b1b8-50f13aef87dc
void AddBackground(Scene scene)
{
    var entity = new Entity("Background");

    var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
    var filePath = Path.Combine(directory, "background.jpg");
    using var input = File.OpenRead(filePath);
    var texture = Texture.Load(game.GraphicsDevice, input);

    var spriteComponent = new SpriteComponent
    {
        SpriteProvider = new SpriteFromTexture { Texture = texture },
    };

    entity.Add(spriteComponent);
    entity.Transform.Position.Z = -500;
    entity.Transform.Position.Y = 12.4f;

    entity.Scene = scene;
}

void Add3DBoxes(Scene scene, int count = 5)
{
    for (int i = 0; i < count; i++)
    {
        var entity = game.CreatePrimitive(PrimitiveModelType.Cube, size: boxSize);

        entity.Name = "Cube";
        entity.Transform.Position = new Vector3(0.5f, 8, 0);
        entity.Scene = scene;
    }
}

void Add2DBoxes(Scene rootScene, int count = 5)
{
    for (int i = 1; i <= count; i++)
    {
        var entity = new Entity
        {
            Name = "Cube",
            Transform = { Position = new(Random.Shared.Next(-5, 5), 5 + Random.Shared.Next(0, 5), 0) }
        };

        entity.Add(new ModelComponent { Model = model });
        //entity.Add(new StaticColliderComponent());
        entity.Add(new RigidbodyComponent()
        {
            IsKinematic = false,
            //ColliderShapes = { new BoxColliderShapeDesc() { Size = new Vector3(1), Is2D = true } },
            ColliderShape = new BoxColliderShape(true, new Vector3(boxSize.X, boxSize.X, 0))
            {
                LocalOffset = new Vector3(boxSize.X / 2, boxSize.X / 2, 0)
            }
        });

        entity.Scene = rootScene;
    }
}

// Another issue
static void AddSpriteBatchRenderer(Scene rootScene)
{
    var entity = new Entity("SpriteBatchRendererEntity", new(1, 1, 1))
    {
        new SpriteBatchRenderer()
    };

    entity.Scene = rootScene;
}