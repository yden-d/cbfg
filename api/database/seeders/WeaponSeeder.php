<?php

namespace Database\Seeders;

use App\Models\Weapon;
use Illuminate\Database\Seeder;

class WeaponSeeder extends Seeder
{
	/**
	 * Run the database seeds.
	 *
	 * @return void
	 */
	public function run()
	{
		foreach (Weapon::WEAPONS as $class => $weaponNames)
		{
			foreach ($weaponNames as $name)
			{
				Weapon::firstOrCreate([
					'class' => $class,
					'name'=> $name,
				])->save();
			}
		}
	}
}
