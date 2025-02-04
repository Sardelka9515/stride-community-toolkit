using Example08_DebugShapes.Scripts;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Rendering.Compositing;
using Stride.Engine;

using var game = new Game();

game.Run(start: Start);

void Start(Scene rootScene)
{
    AddDebugComponent(rootScene);
    SetupBaseScene();
}

void SetupBaseScene()
{
    game.AddGraphicsCompositor()
        .AddImmediateDebugRenderFeature();
    game.Add3DCamera().Add3DCameraController();
    game.AddDirectionalLight();
    game.AddSkybox();
    game.Add3DGround();
}

void AddDebugComponent(Scene scene)
{
    var entity = new Entity("Debug Shapes")
    {
        new ShapeUpdater()
    };
    scene.Entities.Add(entity);
}