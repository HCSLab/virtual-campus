// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


float _ExplosionRate;
float3 _ExplosionCenter;
float _ExplosionRotate;

static const float HalfPI = 1.570795f;

float4 QuaternionFromAxisAngle(float3 axis, float angle)
{
	float halfAngle = angle * 0.5f;
	return float4(axis * sin(halfAngle), cos(halfAngle));
}

float3 TransformByQuaternion(float3 position, float4 quaternion)
{
	float4 Q = quaternion;
	float3 v = position;

	return
		(2.0f * Q.w * Q.w - 1.0f) * v +
		(2.0f * dot(v, Q.xyz) * Q.xyz) +
		(2.0f * Q.w * cross(Q.xyz, v));
}

void transform(inout appdata_full v)
{
	float3 center = v.vertex + v.tangent.xyz;
	float velocity = v.tangent.w;

	float3 move = center - _ExplosionCenter;
	move.x += sin(velocity);
	move.y += atan(velocity);
	move.z += cos(velocity);
	float3 normal = normalize(move);

	float3 dir = v.vertex.xyz - center;
	float4 quaternion;
	{
		float3 axis = cross(normal, float3(0.0001f, -sign(normal.y), 0.0001f));
		axis = normalize(axis);
		quaternion = QuaternionFromAxisAngle(axis, velocity * _ExplosionRotate * _ExplosionRate);
		dir = TransformByQuaternion(dir, quaternion);
	}

	float power = HalfPI * (1.0f - _ExplosionRate);
	v.vertex.xyz = center + dir * sin(power);
	v.vertex.xyz += normal * velocity * cos(power);

	v.normal = TransformByQuaternion(v.normal, quaternion);
}
