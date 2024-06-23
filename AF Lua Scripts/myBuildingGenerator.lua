-- Randomize the dimensions of the building
sizeX = math.random(1, 6) -- Random size on X axis from 1 to 6
sizeZ = math.random(1, 6) -- Random size on Z axis from 1 to 6
floors = math.random(1, 3) -- Random number of floors from 1 to 3
windows = math.random(2, 10) -- Random number of windows from 2 to 10

-- Clear previous model
forge.clear()

-- Arrays of random windows and quoins
randomWindows = { "Walls/window_closed", "Walls/window_squareGlass", "Walls/window_squareGlassLarge", "Walls/window_decorativeLarge" }
randomWindow  = randomWindows[math.random(#randomWindows)]

randomQuoins  = { "Walls/quoin_normal", "Walls/quoin_detailed", "Walls/quoin_diagonal", "Walls/quoin_long", "Walls/quoin_simple" }
randomQuoin   = randomQuoins[math.random(#randomQuoins)]

-- Place the doorway and door together on the ground floor
local doorPlaced = false

for floor = 0, floors - 1 do -- Every floor...

    for x = 0, sizeX - 1 do -- Repeat size on X axis
    
        local buildWall = "Walls/wall"
        if floor == 0 then 
            buildWall = "Walls/wall_border" 
        end -- Place wall border on bottom
        
        if floor > 0 and x % 2 == 0 then -- Place windows at even walls
        
            buildWall = randomWindow
            
            if math.random(0, 2) == 0 then forge.build("City/balcony_basic", {x, floor, -1}, {0, 180, 0}) end -- Add a balcony
            if math.random(0, 2) == 0 then forge.build("City/balcony_basic", {x, floor, sizeZ}, {0, 0, 0}) end -- Add a balcony
        
        end

        if floor == 0 and x == math.floor(sizeX / 2) and not doorPlaced then
            -- Place doorway and door in the center of the ground floor
            forge.build("Walls/doorway", {x, floor, 0}, {0, 0, 0}) -- Place doorway
            forge.build("Buildings/door", {x, floor, 0}, {0, 0, 0}) -- Place door
            doorPlaced = true
        else
            forge.build(buildWall, {x, floor, 0}, {0, 0, 0}) -- Place wall
        end
        forge.build(buildWall, {x, floor, sizeZ}, {0, 0, 0}) -- Place wall
        
        if x == 0 then forge.build(randomQuoin, {sizeX - 1, floor, sizeZ - 1}, {0, 90, 0}) end -- Place quoin on corner
        if x == sizeX - 1 then forge.build(randomQuoin, {sizeX - 1, floor, 0}, {0, 180, 0}) end -- Place quoin on corner
        
        if floor == floors - 1 then
        
            forge.build("Walls/cornice_extended", {x, floor + 0.25, -1}, {0, 180, 0}) -- Place cornice
            forge.build("Walls/cornice_extended", {x, floor + 0.25, sizeZ}, {0, 0, 0}) -- Place cornice
        
        end
        
    end
    
    for z = 0, sizeZ - 1 do -- Repeat size on Z axis
    
        local buildWall = "Walls/wall"
        if floor == 0 then buildWall = "Walls/wall_border" end -- Place wall border on bottom
        
        if floor > 0 and z % 2 == 0 then buildWall = randomWindow end
    
        forge.build(buildWall, {0, floor, z}, {0, 90, 0}) -- Place wall
        forge.build(buildWall, {sizeX, floor, z}, {0, 90, 0}) -- Place wall
        
        if z == 0 then forge.build(randomQuoin, {0, floor, z}, {0, -90, 0}) end -- Place quoin on corner
        if z == sizeZ - 1 then forge.build(randomQuoin, {0, floor, z}, {0, 0, 0}) end -- Place quoin on corner
        
        if floor == floors - 1 then
        
            forge.build("Walls/cornice_extended", {-1, floor + 0.25, z}, {0, -90, 0}) -- Place cornice
            forge.build("Walls/cornice_extended", {sizeX, floor + 0.25, z}, {0, 90, 0}) -- Place cornice
        
        end
        
    end
    
end

-- Place the roof
for x = 0, sizeX - 1 do
    for z = 0, sizeZ - 1 do
    
        forge.build("Roofs/roof_flat", {x, floors, z}, {0, 0, 0}) -- Place roof
        
        local roofItem = math.random(0, 10)
        
        if roofItem == 0 then forge.build("City/roof_windowGlass", {x, floors + 0.1, z}, {0, 0, 0}) end
        if roofItem == 1 then forge.build("City/detail_ventSmall", {x, floors + 0.1, z}, {0, math.random(0, 360), 0}) end
    
    end
end