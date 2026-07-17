using Godot;
using Godot.Collections;
using System;
using System.Drawing;
using static Godot.Image;

public partial class WeaponBuilder : Node3D
{
    
    [Export]
    public float GridSize { get; set; } = 1f;

    private PackedScene SelectedPart;
    public int SelecedIndex;
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

    private Button BuildButton;

    public FiringPipeLine[] WeaponPipeLineArray;


    [Export]
    public Array<PackedScene> AvailableParts { get; set; }
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

        BuildButton = GetNode<Button>("Gui/Button");

        SelectPart(AvailableParts[0]);
        SelecedIndex = 0;


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        RotatePart();
    }
    public override void _PhysicsProcess(double delta)
    {

        (Vector3 Position,GunPart TargetPartBase, Vector3 Normal) = CamRayCaster.MouseRayCast();

        Position = SnapToGrid(Position);
        Vector3 TargetPos = TargetPartBase.GetParentNode3D().Position;
        //fuck this god forsaken spaggeti code.
        //take care of all 6 cases separetly(combining them coused problems with wierd edge cases).
        if (Normal.X > 0.9) {
            
            Position.X = GetPreviewSizeOffset(Normal).X + 0.5f* Normal.X + TargetPos.X;
        }
        if (Normal.X < -0.9) {
            Position.X = GetPreviewSizeOffset(Normal).X + (TargetPartBase.size.X - 0.5f) * Normal.X+ TargetPos.X;
        }
        if (Normal.Y > 0.9) {
            Position.Y = GetPreviewSizeOffset(Normal).Y + 0.5f * Normal.Y+ TargetPos.Y;
        }
        if (Normal.Y < -0.9)
        {
            Position.Y = GetPreviewSizeOffset(Normal).Y + (TargetPartBase.size.Y - 0.5f) * Normal.Y+ TargetPos.Y;

        }
        if (Normal.Z > 0.9)
        {
            Position.Z = GetPreviewSizeOffset(Normal).Z + 0.5f * Normal.Z+ TargetPos.Z;
            
        }
        if (Normal.Z < -0.9)
        {
            Position.Z = GetPreviewSizeOffset(Normal).Z + (TargetPartBase.size.Z - 0.5f) * Normal.Z+ TargetPos.Z;
        }
        


        //apply position and rotation
        PlacePreviewOrigin.Position = Position;

        PlacePreviewPivot.RotationDegrees = PlaceRotation;
        //place part
        if (Input.IsActionJustPressed("fire_weapon")&& !PlaceCollider.HasOverlappingBodies())
        {

            PlacePart(Position, PlaceRotation);
        }
        //switch part(TEMPORARY, replace with side menu)
        if (Input.IsActionJustPressed("change_selected_part")) {
            SelecedIndex++;
            SelecedIndex = SelecedIndex % AvailableParts.Count;
            SelectPart(AvailableParts[SelecedIndex]);
        }
    }

    public float SnapToGrid(float Value) {
        int intValue = (int)(Value);
        return intValue + ((Value - intValue > 0.49) ? 1 : 0);
    }
    public Vector3 SnapToGrid(Vector3 Value) {
        return new Vector3(SnapToGrid(Value.X), SnapToGrid(Value.Y), SnapToGrid(Value.Z));
    }

    public void PlacePart(Vector3 Position,Vector3 Rotation) {
        Node3D Part = (Node3D)SelectedPart.Instantiate();
        PartContainer.AddChild(Part);
        Part.Position = Position;
        Part.RotationDegrees = Rotation;

        //connect to Build signal if needed
        if (Part.HasMethod("ConnectBuildButton")) {
            Part.Call("ConnectBuildButton", BuildButton, WeaponPipeLineArray);
        }
    }

    public void SelectPart(PackedScene PartScene)
    {
        SelectedPart = PartScene;
        ChangePreview(PartScene);
    }
    public void ChangePreview(PackedScene PartScene)
    {
        //remove previous preview
        if (PlacePreview.GetChildCount() > 1) {
            PlacePreview.GetChild(1).QueueFree();
        }
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
    //calculate needed offset for the current part's size and rotation. 
    public Vector3 GetPreviewSizeOffset(Vector3 Normal) {
        //GD.Print(Normal + "," + PartPreview.GlobalBasis.X.Normalized());
        if (EqualApprox(PartPreview.GlobalBasis.X.Normalized(),Normal)) {
            //GD.Print("Z");
            
            return Normal*(PartPreviewBase.size.X - 0.5f);
        }
        if (EqualApprox(PartPreview.GlobalBasis.Y.Normalized(), Normal))
        {
            return Normal * (PartPreviewBase.size.Y - 0.5f);
        }
        
        if (EqualApprox(PartPreview.GlobalBasis.Z.Normalized(), Normal))
        {

            return Normal * (PartPreviewBase.size.Z - 0.5f);
        }
        return new Vector3(0.5f, 0.5f, 0.5f) * Normal;
    }

    //effectively Vector3.IsEqualApprox but with a custome torarence. encounterd problems with rounding errors.
    public bool EqualApprox(Vector3 Vec1, Vector3 Vec2,float tolerance=0.001f) {
        return Vec1.DistanceSquaredTo(Vec2) < tolerance * tolerance;
    }
}
