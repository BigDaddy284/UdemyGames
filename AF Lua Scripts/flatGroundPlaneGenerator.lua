-- Ground plane generator for Asset Forge

sizeX = math.random(1, 8) -- Random size on X axis from 1 to 8
sizeZ = math.random(1, 8) -- Random size on Z axis from 1 to 8

forge.clear() -- Clear previous model

-- Function to generate height variations with constraints to avoid extreme spikes
function generateHeights(sizeX, sizeZ)
    heights = {}
    for x = 0, sizeX do
        heights[x] = {}
        for z = 0, sizeZ do
            if x == 0 or x == sizeX or z == 0 or z == sizeZ then
                heights[x][z] = 0 -- Ensure edges are flat for easier connection to other tiles
            else
                heights[x][z] = math.random() * math.random(-0.5, 0.5) -- Random heights between -0.5 and 0.5
            end
        end
    end
    return heights
end

-- Function to smooth the heights to avoid extreme variations
function smoothHeights(heights, sizeX, sizeZ)
    for x = 1, sizeX - 1 do
        for z = 1, sizeZ - 1 do
            local average = (heights[x-1][z] + heights[x+1][z] + heights[x][z-1] + heights[x][z+1]) / 4
            heights[x][z] = (heights[x][z] + average) / 2
        end
    end
end

-- Function to build the ground plane
function buildGroundPlane(heights, sizeX, sizeZ)
    for x = 0, sizeX - 1 do
        for z = 0, sizeZ - 1 do
            local h1 = heights[x][z]
            local h2 = heights[x+1][z]
            local h3 = heights[x][z+1]
            local h4 = heights[x+1][z+1]

            -- Calculate the average height for placing the tile
            local averageHeight = (h1 + h2 + h3 + h4) / 4

            -- Build plane segments with height variations
            forge.build("modular_terrain_collection/Hilly_Terrain_Grass_Floor", {x, averageHeight, z}, {0, 0, 0})
        end
    end
end

-- Main execution
heights = generateHeights(sizeX, sizeZ)
smoothHeights(heights, sizeX, sizeZ)
buildGroundPlane(heights, sizeX, sizeZ)
