using UnityEngine;
using System.Collections.Generic;

public class BoidsController : MonoBehaviour
{
    private Vector3 m_Vector3Zero;
	public GameObject m_BoidPrefab;
	public Transform m_Target;

    private void Awake() {
        Initialize();
    }

	Vector3 cohesion;
	Vector3 separation;
	Vector3 alignment;
	Vector3 destination;

	private List<BoidsUnit> m_Boids;

    private void Initialize() {
        m_Vector3Zero = new Vector3(0, 0, 0);
        m_Boids = new List<BoidsUnit>();
    }

	private void Start() {
		Spawn();
	}

	private void Spawn() {
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 10; j++) {
				GameObject boid = Instantiate(m_BoidPrefab);
				BoidsUnit boidUnit = boid.GetComponent<BoidsUnit>();
				boidUnit.Transform.position = new Vector3(i * 4.0f, Random.Range(0.0f, 10.0f), j * 4.0f);
				boidUnit.m_Speed = new Vector3(0, 0, 0);
				m_Boids.Add(boidUnit);
			}
		}
	}

	private void Update() {
		Move();
	}

	public void Move() {
		BoidsUnit unit;
		Vector3 speed;

		for (int i = 0; i < m_Boids.Count; i++) {
			unit = m_Boids[i];
			speed = unit.m_Speed;

			cohesion = CohesionForUnit(unit, GetNearestBoids(unit)).normalized * 0.01f;
			separation = SeparationForUnit(unit, GetNearestBoids(unit)).normalized * 0.1f;
			alignment = AlignmentForUnit(unit, GetNearestBoids(unit)).normalized * 0.01f;

			cohesion = Vector3.ClampMagnitude(cohesion, 0.1f);
			separation = Vector3.ClampMagnitude(separation, 0.1f);
			alignment = Vector3.ClampMagnitude(alignment, 0.1f);

			destination = DestinationForUnit(unit, m_Target.position).normalized * 1.0f;
			speed += cohesion + separation + alignment + destination;

			unit.m_Speed = speed;
			unit.Transform.Translate(unit.m_Speed * Time.deltaTime);
		}
	}

	private Vector3 CohesionForUnit(BoidsUnit unit, List<BoidsUnit> nearestUnits) {
		Vector3[] positions = new Vector3[nearestUnits.Count];
        for (int i = 0; i < positions.Length; i++) {
            positions[i] = nearestUnits[i].Transform.position;
        }

        return ((Vector3Average(positions)) - unit.Transform.position);
    }
	private Vector3 SeparationForUnit(BoidsUnit unit, List<BoidsUnit> nearestUnits) {
		Vector3[] directions = new Vector3[nearestUnits.Count];
        for (int i = 0; i < directions.Length; i++) {
            directions[i] = nearestUnits[i].Transform.position - unit.Transform.position;
        }

        return Vector3Average(directions);
    }

	private Vector3 AlignmentForUnit(BoidsUnit unit,  List<BoidsUnit> nearestUnits) {
		Vector3[] speeds = new Vector3[nearestUnits.Count];
        for (int i = 0; i < speeds.Length; i++) {
            speeds[i] = nearestUnits[i].m_Speed;
        }

        return Vector3Average(speeds);
    }

    private Vector3 DestinationForUnit(BoidsUnit unit, Vector3 Target) {
        return Target - unit.Transform.position;
    }

    private Vector3 Vector3Average(params Vector3[] vectors) {
        int vectorsCount = vectors.Length;
        Vector3 average = m_Vector3Zero;

        for (int i = 0; i < vectorsCount; i++) {
            average += vectors[i];
        }

        return average / vectorsCount;
    }

	private List<BoidsUnit> GetNearestBoids(BoidsUnit boid) {
		List<BoidsUnit> nearestBoids = new List<BoidsUnit>();

		for (int i = 0; i < m_Boids.Count; i++) {
			if (m_Boids[i] == boid)
				continue;

			if (IsPointInCircle(m_Boids[i].Transform.position, boid.Transform.position, 10)) {
				nearestBoids.Add(m_Boids[i]);
			}
		} 	

		return nearestBoids;
	}

	public bool IsPointInCircle(Vector3 point, Vector3 circleCenter, float radius) {
		return (Mathf.Pow((circleCenter.x - point.x), 2) + Mathf.Pow((circleCenter.z - point.z), 2)) <= radius * radius;
	}
}
