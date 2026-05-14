using Godot;
using System;
using System.Drawing;
using static Godot.Image;

public partial class WeaponBuilder : Node3D
{
    
    [Export]
    public float GridSize { get; set; } = 1f;

    public PackedScene SelectedPart;
    public Node3D PlacePreview;
    public Vector3 PlaceRotation=Vector3.Zero;

    private Node3D PlacePreviewOrigin;
    private Node3D PlacePreviewPivot;
    private Node3D PartPreview;
    private GunPart PartPreviewBase;
    private Area3D PlaceCollider;
    private BoxShape3D PlaceColliderShape;
    private CameraRayCaster CamRayCaster;
    private Node3D PartContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CamRayCaster = GetNode<CameraRayCaster>("CameraPivot/CameraPivotY/Camera");
        PartContainer = GetNode<Node3D>("PartContainer");
        PlacePreviewOrigin= GetNode<Node3D>("PartPreviewOrigin");
        PlacePreviewPivot= GetNode<Node3D>("PartPreviewOrigin/PartPreviewPivot");
        PlacePreview = GetNode<Node3D>("PartPreviewOrigin/PartPreviewPivot/PartPreview");
        PlaceCollider = GetNode<Area3D>("PartPreviewOrigin/PartPreviewPivot/PartPreview/PlaceCollider");
        PlaceColliderShape = (BoxShape3D)GetNode<CollisionShape3D>("PartPreviewOrigin/PartPreviewPivot/PartPreview/PlaceCollider/PlaceColliderShape").Shape;
        SelectedPart = GD.Load<PackedScene>("res://Scenes/GunParts/test_part.tscn");
        ChangePreview(SelectedPart);
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        RotatePart();
    }
    public override void _PhysicsProcess(double delta)
    {

        (Vector3 Position,GunPart TargetPartBase, Vector3 Normal) = CamRayCaster.MouseRayCast();
        Position += Normal* ((Normal.X==-1|| Normal.Y == -1|| Normal.Z == -1) ?TargetPartBase.size:Vector3.One);

        
        Position +=ApplyRotationOffset(Normal);
        Position = SnapToGrid(Position);
        
        PlacePreviewOrigin.Position = Position;
        PlacePreviewPivot.RotationDegrees = PlaceRotation;
        if (Input.IsActionJustPressed("fire_weapon")&& !PlaceCollider.HasOverlappingBodies())
        {

            PlacePart(Position, PlaceRotation);
        }
    }

    public float SnapToGrid(float Value) {
        return ((int)(Value / GridSize)) * GridSize;// + ((Value % GridSize >= GridSize *(0.5))?GridSize : 0);
    }
    public Vector3 SnapToGrid(Vector3 Value) {
        return new Vector3(SnapToGrid(Value.X), SnapToGrid(Value.Y), SnapToGrid(Value.Z));
    }

    public void PlacePart(Vector3 Position,Vector3 Rotation) {
        Node3D Part = (Node3D)SelectedPart.Instantiate();
        PartContainer.AddChild(Part);
        Part.Position = Position;
        Part.RotationDegrees = Rotation;
    }

    public void ChangePreview(PackedScene PartScene)
    {
        PartPreview = (Node3D)PartScene.Instantiate();//create preview
        PlacePreview.AddChild(PartPreview);
        PartPreviewBase = PartPreview.GetNode<GunPart>("GunPartBase");//get partBase node
        CamRayCaster.SetPreviewExclusion(PartPreviewBase);
        PlaceColliderShape.Size = PartPreviewBase.size-new Vector3(0.1f,0.1f,0.1f);//silgtly smaller to allow for the parts to touch
        PlaceCollider.Position = PartPreviewBase.Position*PartPreview.Scale;//ajust for scale
        PartPreviewBase.DisableCollision();
    }

    public void RotatePart() {
        if (Input.IsActionJustPressed("builder_rotate_x_positive"))
        {
            PlaceRotation += new Vector3(90, 0, 0);
        }
        if (Input.IsActionJustPressed("builder_rotate_x_negative"))
        {
            PlaceRotation += new Vector3(-90, 0, 0);
        }
        if (Input.IsActionJustPressed("builder_rotate_y_positive"))
        {
            PlaceRotation += new Vector3(0, 90, 0);
        }
        if (Input.IsActionJustPressed("builder_rotate_y_negative"))
        {
            PlaceRotation += new Vector3(0, -90, 0);
        }
        if (Input.IsActionJustPressed("builder_rotate_z_positive"))
        {
            PlaceRotation += new Vector3(0, 0, 90);
        }
        if (Input.IsActionJustPressed("builder_rotate_z_negative"))
        {
            PlaceRotation += new Vector3(0, 0, -90);
        }
    }

    public Vector3 ApplyRotationOffset(Vector3 Normal) {
        Vector3 Offset = Vector3.Zero;
        if (Normal.IsEqualApprox(PlacePreviewPivot.Basis.X))
        {
            Offset += PartPreviewBase.size.X* Normal.Abs()- Normal.Abs();
        }
        else if (Normal.IsEqualApprox(PlacePreviewPivot.Basis.Y))
        {
            Offset += Normal*(PartPreviewBase.size.Y-1);
        }
        else if (Normal.IsEqualApprox(PlacePreviewPivot.Basis.Z))
        {
            Offset += PartPreviewBase.size.Z * Normal.Abs()- Normal.Abs();

        }
        
        return Offset;
    }
}
