VoxelCore non-overlap unwrap

propmaker.obj — merged mesh, atlas UV (vt = geometry uv, PNG top-left)
propmaker.mtl — material with diffuse map
voxel_diffuse.png — packed diffuse atlas (2048×2048)
voxel_normal.png — tangent-space normal atlas (linear RGB, map_Bump in MTL)
voxel_metalness.png — metalness atlas (linear, map_Pm in MTL)
voxel_roughness.png — roughness atlas (linear, map_Pr in MTL)
voxel_shadow.png — baked AO/shadow (white = light, multiply over diffuse)
voxel_curvature.png — experimental curvature from normal atlas (white = edges)

UV islands: translate + uniform scale only (no rotation — tangent maps).
Island pack size = world span × scene target texel density (uniform among primary; contact/low ×0.15).
Contact/joint islands are packed smaller to free texels for visible surfaces.