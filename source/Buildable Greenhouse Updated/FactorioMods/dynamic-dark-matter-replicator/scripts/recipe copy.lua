
function initialize_best_machine()
	best_machine = {}
	
	return best_machine
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

function getCategory(category)
	
end

function rawingredients(recipe, exclude) -- call this function
	ret = getRawIngredients(recipe, exclude, best_machine)
	--ret.time = (ret.time or 0) + (recipe.energy or 1) / best_machine[recipe.category].speed
	--ret.energy = (ret.energy or 0) +
		--(recipe.energy or 1) / best_machine[recipe.category].speed * best_machine[recipe.category].energy
	return ret
end

function getIngredients(recipe)
	local ingredients = {}
	for i, ingredient in pairs(recipe.ingredients) do
		if (ingredient.name and ingredient.amount) then
			ingredients[ingredient.name] = ingredient.amount
		elseif (ingredient[1] and ingredient[2]) then
			ingredients[ingredient[1]] = ingredient[2]
		end
	end
	return ingredients
end

function getProducts(recipe)
	local products = {}
	if(recipe.result) then
		products[recipe.result] = 1
	elseif (recipe.results) then
		for i, product in pairs(recipe.results) do
			if (product.name and product.amount) then
				products[product.name] = product.amount
			elseif product.amount_min and product.amount_max then
				products[product.name] = (product.amount_min + product.amount_max) / 2 * (product.probability or 1)
			end
		end
	end
	return products
end

function getRecipes(item)
	local recipes = {}
	for i, recipe in pairs(data.raw.recipe) do
		if i ~= "coal-liquefaction" and
			not
			(
			string.sub(item, -7) ~= "-barrel" and string.sub(i, -7) == "-barrel" and
				(string.sub(i, 1, 5) == "fill-" or string.sub(i, 1, 6) == "empty-")) then
			local products = getProducts(recipe)
			for product, amount in pairs(products) do
				if (product == item) then
					table.insert(recipes, recipe)
				end
			end
		end
	end
	return recipes
end

function getRawIngredients(recipe, excluded, best_machine)
	local raw_ingredients = {
		["time"] = 0,
		["energy"] = 0
	}
	for name, amount in pairs(getIngredients(recipe)) do
		-- Do not use an item as its own ingredient
		if (excluded[name]) then
			return { ERROR_INFINITE_LOOP = name }
		end
		local excluded_ingredients = { [name] = true }
		for k, v in pairs(excluded) do
			excluded_ingredients[k] = true
		end

		-- Recursively find the sub-ingredients for each ingredient
		-- There might be more than one recipe to choose from
		local subrecipes = {}
		local loop_error = nil
		for i, subrecipe in pairs(getRecipes(name)) do
			local subingredients = getRawIngredients(subrecipe, excluded_ingredients, best_machine)
			if (subingredients.ERROR_INFINITE_LOOP) then
				loop_error = subingredients.ERROR_INFINITE_LOOP
			else
				local value = 0
				for subproduct, subamount in pairs(getProducts(subrecipe)) do
					value = value + subamount
				end

				local divisor = 0
				for subingredient, subamount in pairs(subingredients) do
					if subingredient ~= "intermediates" then
						divisor = divisor + subamount
					end
				end

				if (divisor == 0) then divisor = 1 end

				table.insert(subrecipes, { recipe = subrecipe, ingredients = subingredients, value = value / divisor })
			end
		end

		if (#subrecipes == 0) then
			if (loop_error and loop_error ~= name) then
				-- This branch of the recipe tree is invalid
				return { ERROR_INFINITE_LOOP = loop_error }
			else
				-- This is a raw resource
				if (raw_ingredients[name]) then
					raw_ingredients[name] = raw_ingredients[name] + amount
				else
					raw_ingredients[name] = amount
				end

			end
		else
			-- Pick the cheapest recipe
			local best_recipe = nil
			local best_value = 0
			for i, subrecipe in pairs(subrecipes) do
				if (best_value < subrecipe.value) then
					best_value = subrecipe.value
					best_recipe = subrecipe
				end
			end

			local multiple = 0
			for subname, subamount in pairs(getProducts(best_recipe.recipe)) do
				multiple = multiple + subamount
			end

			if not raw_ingredients["intermediates"] then raw_ingredients["intermediates"] = {} end
			for subname, subamount in pairs(best_recipe.ingredients) do
				if subname == "intermediates" then
					for a, b in pairs(subamount) do
						raw_ingredients["intermediates"][a] = (raw_ingredients["intermediates"][a] or 0) + b * amount / multiple
					end
				elseif (raw_ingredients[subname]) then
					raw_ingredients[subname] = raw_ingredients[subname] + amount * subamount / multiple
				else
					raw_ingredients[subname] = amount * subamount / multiple
				end
			end
			for a, b in pairs(getProducts(best_recipe.recipe)) do
				raw_ingredients["intermediates"][a] = (raw_ingredients["intermediates"][a] or 0) + b * amount / multiple
			end
			category = getCategory(best_recipe.recipe.category)
			raw_ingredients["time"] = raw_ingredients["time"] +
				(best_recipe.recipe.energy_required or 1) / best_machine[category].speed * amount / multiple
			raw_ingredients["energy"] = raw_ingredients["energy"] +
				(best_recipe.recipe.energy_required or 1) / best_machine[category].speed *
				best_machine[category].energy * amount / multiple

		end
	end

	return raw_ingredients
end