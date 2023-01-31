local exclude = {
	["used-up-uranium-fuel-cell"] = true,
}

local best_machine = {}

local furnaces = {}

local recipe_table = {}

function initialize_best_machine()
	for name, assem in pairs(data.raw["assembling-machine"]) do
		if assem.crafting_categories and assem.energy_usage then
			for _, cat in pairs(assem.crafting_categories) do
				if not best_machine[cat] then
					best_machine[cat] = { speed = assem.crafting_speed or 1, energy = getEnergy(assem.energy_usage), name = name }
				elseif (assem.crafting_speed or 1) / (getEnergy(assem.energy_usage) or 1) >
					best_machine[cat].speed / best_machine[cat].energy then
					best_machine[cat] = { speed = assem.crafting_speed or 1, energy = getEnergy(assem.energy_usage), name = name }
				end
			end
		end
	end
	
end

function initalize_furnace()
	for name, furnace in pairs(data.raw.furnace) do
		furnaces[name] = { speed = furnace.crafting_speed or 1, energy = getEnergy(furnace.energy_usage), name = name}
	end
end

function getEnergy(energy)
	if string.find(energy, "k") or string.find(energy, "K") then
		return tonumber(string.sub(energy, 1, #energy - 2)) * 10 ^ 3
	elseif string.find(energy, "M") then
		return tonumber(string.sub(energy, 1, #energy - 2)) * 10 ^ 6
	elseif string.find(energy, "G") then
		return tonumber(string.sub(energy, 1, #energy - 2)) * 10 ^ 9
	elseif string.find(energy, "T") then
		return tonumber(string.sub(energy, 1, #energy - 2)) * 10 ^ 12
	elseif string.find(energy, "P") then
		return tonumber(string.sub(energy, 1, #energy - 2)) * 10 ^ 15
	elseif string.find(energy, "E") then
		return tonumber(string.sub(energy, 1, #energy - 2)) * 10 ^ 18
	elseif string.find(energy, "Z") then
		return tonumber(string.sub(energy, 1, #energy - 2)) * 10 ^ 21
	elseif string.find(energy, "Y") then
		return tonumber(string.sub(energy, 1, #energy - 2)) * 10 ^ 24
	else
		return 0
	end
end

initialize_best_machine()
initalize_furnace()

function getIngredients(recipe)
	local ingredients = {}
	if recipe.ingredients then
		for i, ingredient in pairs(recipe.ingredients) do
			if (ingredient.name and ingredient.amount) then
				ingredients[ingredient.name] = ingredient.amount
			elseif (ingredient[1] and ingredient[2]) then
				ingredients[ingredient[1]] = ingredient[2]
			end
		end
	else
		local expensive = {}
		for i, ingredient in pairs(recipe.expensive.ingredients) do
			if (ingredient.name and ingredient.amount) then
				expensive[ingredient.name] = ingredient.amount
			elseif (ingredient[1] and ingredient[2]) then
				expensive[ingredient[1]] = ingredient[2]
			end
		end
		ingredients["expensive"] = expensive

		local normal = {}
		for i, ingredient in pairs(recipe.normal.ingredients) do
			if (ingredient.name and ingredient.amount) then
				normal[ingredient.name] = ingredient.amount
			elseif (ingredient[1] and ingredient[2]) then
				normal[ingredient[1]] = ingredient[2]
			end
		end
		ingredients["normal"] = normal
	end
	return ingredients
end

function getRawIngredients(recipe)
	local ingredients = getIngredients(recipe)
end

--log(serpent.block(data.raw.recipe["advanced-circuit"]))
--log(serpent.block(data.raw.recipe["iron-plate"]))
--log(serpent.block(data.raw.recipe["steel-plate"]))

--log(serpent.block(getRawIngredients(data.raw.recipe["advanced-circuit"])))

for name,recipe in pairs(data.raw.recipe) do
	log(serpent.block(getRawIngredients(recipe)))
end