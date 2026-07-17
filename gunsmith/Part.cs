using Godot;
using System;

public partial class Part : Node3D
{
	public bool[,] ShapeArray;//V y+ , > x+  | MUST BE A SQUARE!!!
	public Vector2I Index;
	public Vector3 GrabPoint;

    [Export]
    public MeshInstance3D mesh { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public void rotateMatrixCCW(bool[,] mat)
    {
        int n = mat.GetLength(0);

        // Consider all cycles one by one
        for (int i = 0; i < n / 2; i++)
        {

            // Consider elements in group of 4
            // as P1, P2, P3 & P4 in current square
            for (int j = i; j < n - i - 1; j++)
            {

                bool temp = mat[i,j];
                mat[i,j] = mat[j,n - 1 - i];
                mat[j,n - 1 - i] = mat[n - 1 - i,n - 1 - j];
                mat[n - 1 - i,n - 1 - j] = mat[n - 1 - j,i];
                mat[n - 1 - j,i] = temp;
            }
        }
    }
    public void rotateMatrixCW(bool[,] mat)
    {
        int n = mat.GetLength(0);

        // Consider all cycles one by one
        for (int i = 0; i < n / 2; i++)
        {

            // Consider elements in group of 4
            // as P1, P2, P3 & P4 in current square
            for (int j = i; j < n - i - 1; j++)
            {

                bool temp = mat[i, j];
                mat[i, j] = mat[n - 1 - j, i];
                mat[n - 1 - j, i] = mat[n - 1 - i, n - 1 - j];
                mat[n - 1 - i, n - 1 - j] = mat[j, n - 1 - i];
                mat[j, n - 1 - i] = temp;
            }
        }
    }
    public void RotateCCW() {
        rotateMatrixCCW(ShapeArray);
        mesh.RotateObjectLocal(Basis.Z, (float)(Math.PI/2));
        DebugPrint(ShapeArray);

    }
    public void RotateCW()
    {
        rotateMatrixCW(ShapeArray);
        mesh.RotateObjectLocal(Basis.Z, -(float)(Math.PI / 2));
        DebugPrint(ShapeArray);
    }
    public void DebugPrint(bool[,] arr) {
        string printPayload = "";
        int size = arr.GetLength(0);
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                printPayload += arr[i, j] + ",";
            }
            printPayload += "\n";
        }
        GD.Print(printPayload);
    }
}
