<?php

namespace Database\Seeders;

use App\Models\Skill;
use App\Models\Weapon;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class SkillSeeder extends Seeder
{
	/**
	 * Array representation of the skill tree.
	 * New skills and weapons should be added to this tree and the seeder should be run.
	 * The first level of the tree is keyed by the fighter classes.
	 * The second level is keyed by the weapon name, and each subsequent level
	 * is keyed by the skill name.
	 * Each skill array must specify the controller binding under the 'binding' key.
	 * Each skill array may also have a skill subarray keyed by 'children', containing the
	 * skills directly dependent on the skill.
	 */
	private const TREE = [
		Weapon::CLASS_STRENGTH => [
			Weapon::WEAPON_SWORD => [
				// Stab
				'Stab' => [
					'binding' => Skill::BINDING_SIDE_LIGHT,
					'children' => [
						// Lunge
						'Lunge' => ['binding' => Skill::BINDING_SIDE_HEAVY],
					],
				],
				// Swipe
				'Swipe' => [
					'binding' => Skill::BINDING_NEUTRAL_LIGHT,
					'children' => [
						// Up swipe
						'UpSwipe' => ['binding' => Skill::BINDING_UP_LIGHT],
						// Spin
						'Spin' => [
							'binding' => Skill::BINDING_UP_HEAVY,
							'children' => [
								// Boomerang
								'Boomerang' => [
									'binding' => Skill::BINDING_NEUTRAL_HEAVY,
								],
							],
						],
					],
				],
			],
			Weapon::WEAPON_SHIELD => [
				// Block
				'Block' => [
					'binding' => Skill::BINDING_NEUTRAL_HEAVY,
					'children' => [
						// Up block
						'UpBlock' => ['binding' => Skill::BINDING_UP_HEAVY],
					],
				],
				// Bash
				'Bash' => [
					'binding' => Skill::BINDING_SIDE_LIGHT,
					'children' => [
						// Charge
						'Charge' => ['binding' => Skill::BINDING_SIDE_HEAVY],
						// Throw
						'Throw' => [
							'binding' => Skill::BINDING_SIDE_LIGHT,
							'children' => [
								// Up throw
								'UpThrow' => ['binding' => Skill::BINDING_UP_LIGHT],
							],
						],
					],
				],
			],
		],
		Weapon::CLASS_DEXTERITY => [
			Weapon::WEAPON_BOW => [
				// Swipe
				'Swipe' => ['binding' => Skill::BINDING_SIDE_LIGHT],
				// Single shot
				'SingleShot' => [
					'binding' => Skill::BINDING_NEUTRAL_LIGHT,
					'children' => [
						// Exploding shot
						'ExplodingShot' => ['binding' => Skill::BINDING_NEUTRAL_HEAVY],
						// Faster shot
						'FasterShot' => ['binding' => Skill::BINDING_SIDE_HEAVY],
						// Multi shot
						'MultiShot' => [
							'binding' => Skill::BINDING_NEUTRAL_HEAVY,
							'children' => [
								// Single volley
								'SingleVolley' => [
									'binding' => Skill::BINDING_UP_LIGHT,
									'children' => [
										// Multi volley
										'MultiVolley' => ['binding' => Skill::BINDING_UP_HEAVY],
									],
								],
							],
						],
					],
				],
			],
		],
		Weapon::CLASS_INTELLIGENCE => [
			Weapon::WEAPON_WAND => [
				// Fireball
				'Fireball' => [
					'binding' => Skill::BINDING_NEUTRAL_LIGHT,
					'children' => [
						// Firework
						'Firework' => ['binding' => Skill::BINDING_UP_LIGHT],
						// Laser
						'Laser' => ['binding' => Skill::BINDING_UP_HEAVY],
					],
				],
				// Swipe
				'Swipe' => [
					'binding' => Skill::BINDING_SIDE_LIGHT,
					'children' => [
						// Teleport
						'Teleport' => ['binding' => Skill::BINDING_SIDE_HEAVY],
						// Confusion
						'Confusion' => ['binding' => Skill::BINDING_NEUTRAL_HEAVY],
					],
				],
			],
			Weapon::WEAPON_STAFF => [
				// Whack
				'Whack' => ['binding' => Skill::BINDING_SIDE_LIGHT],
				// Ice bolt
				'IceBolt' => [
					'binding' => Skill::BINDING_NEUTRAL_LIGHT,
					'children' => [
						// Vortex
						'Vortex' => ['binding' => Skill::BINDING_NEUTRAL_HEAVY],
						// Gust
						'Gust' => [
							'binding' => Skill::BINDING_UP_LIGHT,
							'children' => [
								// Thunder
								'Thunder' => ['binding' => Skill::BINDING_SIDE_HEAVY],
								// Force Field
								'ForceField' => ['binding' => Skill::BINDING_UP_HEAVY],
							],
						],
					],
				],
			],
		],
	];

	/**
	 * Run the database seeds.
	 *
	 * @return void
	 */
	public function run()
	{
		foreach (self::TREE as $class => $weapons)
		{
			foreach ($weapons as $weaponName => $skills)
			{
				$weaponId = DB::table('weapons')
					->where('name', '=', $weaponName)
					->where('class', '=', $class)
					->first()
					->id;
				foreach ($skills as $skillName => $skill)
				{
					self::addSkill($weaponId, null, $skillName, $skill);
					// TODO add default skills to User
				}
			}
		}
	}

	protected static function addSkill(int $weaponId, int|null $parentId, string $skillName, array $skillArray)
	{
		/** @var Skill */
		$skill = Skill::firstOrCreate([
			'weapon_id' => $weaponId,
			'name' => $skillName,
		], [
			'parent_id' => $parentId,
			'binding' => $skillArray['binding'],
		]);
		// Update parent and binding
		$skill->parent_id = $parentId;
		$skill->binding = $skillArray['binding'];
		$skill->save();

		if (array_key_exists('children', $skillArray))
		{
			foreach ($skillArray['children'] as $childName => $child)
			{
				self::addSkill($weaponId, $skill->id, $childName, $child);
			}
		}
	}
}
