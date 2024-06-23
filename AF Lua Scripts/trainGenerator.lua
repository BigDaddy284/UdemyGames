forge.clear()

trainHeight = math.random(10, 20) / 100
trainWidth = math.random(100, 125) / 100
wheelArchHeight = math.random(0, 10) / 100
wheelHeight = 0.5
windshieldHeight = math.random(50, 100) / 100

forge.build("Trains/body_detail", {0, 1, 0}, {0, 0, 0}, {trainWidth, 1, 1})

randomFrontsCabins = { "Trains/diesel_cabin_front", "Trains/diesel_cabin" }
randomFrontCabins  = randomFrontsCabins[math.random(#randomFrontsCabins)];

forge.build(randomFrontCabins, {0, 1.5, 0}, {0, 0, 0}, {trainWidth, 1, 1})

dieselcabindetail = "Trains/diesel_cabin_detail"
forge.build(dieselcabindetail, {0, 1.5, -1}, {0, 0, 0}, {trainWidth, 1, 1})

randomFronts = { "Trains/diesel_front_slant", "Trains/diesel_front_detailed", "Trains/diesel_front_basic" }
randomFront  = randomFronts[math.random(#randomFronts)];

forge.build(randomFront, {0, 1, 1}, {0, 180, 0}, {trainWidth, 1, 1})

randomFrontLights = { "Trains/detail_light", "Trains/detail_light_square" }
randomFrontLight  = randomFrontLights[math.random(#randomFrontLights)];

forge.build(randomFrontLight, {-0.3, 2, 0.3}, {0, 0, 0}, {trainWidth, 1, 1})
forge.build(randomFrontLight, {0.3, 2, 0.3}, {0, 0, 0}, {trainWidth, 1, 1})


randomBogies = { "Trains/bogie", "Trains/bogie_beam", "Trains/bogie_clamp" }
randomBogie  = randomBogies[math.random(#randomBogies)];

forge.build(randomBogie, {0, 0.5, 1}, {0, 0, 0}, {trainWidth, 1, 1})

forge.build(randomBogie, {0, 0.5, -1}, {0, 0, 0}, {trainWidth, 1, 1})

randomFuels = { "Trains/fuel_large", "Trains/bogie_beam", "Trains/bogie_clamp" }
randomFuel  = randomFuels[math.random(#randomFuels)];

forge.build(randomFuel, {0, 0.5, 0}, {0, 0, 0}, {trainWidth, 1, 1})

randomFuelThinFronts = "Trains/fuel_thin_front"

forge.build(randomFuelThinFronts, {0, 0.6, 1.5}, {0, 0, 0}, {trainWidth, 1.1, 0.7})


randomBacks = { "Trains/body_vents", "Trains/body_vent", "Trains/body_stripe" }
randomBack  = randomBacks[math.random(#randomBacks)];

forge.build(randomBack, {0, 1, -1}, {0, 0, 0}, {trainWidth, 1, 1})


randomWheels = { "Trains/wheels_default", "Trains/wheels_simple" }
randomWheel  = randomWheels[math.random(#randomWheels)];

forge.build(randomWheel, {0, wheelHeight, -1}, {0, 180, 0})
forge.build(randomWheel, {0, wheelHeight,  1}, {0, 180, 0})

randomFrames = { "Trains/bogie_frame", "Trains/bogie_frame_connection" }
randomFrame  = randomFrames[math.random(#randomFrames)];

forge.build(randomFrame, {0, wheelHeight, -1}, {0, 180, 0})
forge.build(randomFrame, {0, wheelHeight,  1}, {0, 180, 0})


