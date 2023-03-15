<?php

namespace App\Http\Controllers;

use App\Models\Loadout;
use App\Models\Skill;
use App\Models\User;
use App\Models\Weapon;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Facades\Validator;

class AuthController extends Controller
{
	public function register(Request $request)
	{
		$validator = Validator::make($request->all(), [
			'email' => [
				'required',
				'email',
				'unique:users',
			],
			'username' => [
				'required',
				'unique:users',
			],
			'password' => [
				'required',
			],
		]);

		if ($validator->fails())
		{
			return response()->json($validator->errors(), 400);
		}

		$user = User::create(array_merge($request->all(), [
			'password' => Hash::make($request->password),
		]));

		// Add default skills
		foreach (Skill::whereNull('parent_id')->get() as $skill)
		{
			$user->addSkill($skill);
		}

		// Add default loadouts
		// foreach (Weapon::all() as $weapon)
		// {
		// 	Loadout::create([
		// 		'user_id' => $user->id,
		// 		'weapon_id' => $weapon->id,
		// 	]);
		// }
		self::createDefaultLoadouts($user);
	}

	public function login(Request $request)
	{
		$validator = Validator::make($request->all(), [
			'email' => [
				'required',
			],
			'password' => [
				'required',
			],
		]);

		if ($validator->fails())
		{
			return response()->json($validator->errors(), 400);
		}

		$credentials = $request->validate([
			'email' => 'required',
			'password' => 'required',
		]);

		if (Auth::attempt($request->all(['email', 'password'])))
		{
			$request->session()->regenerate();
			return;
		}

		return response()->json(['message' => 'Email or password incorrect.'], 401);
	}

	function whoami()
	{
		$user = Auth::user();
		return $user
			?
			User::with('loadoutPrimary', 'loadoutSecondary', 'skills', 'loadouts')->find($user->id)
			:
			response()->json(['message' => 'Not authenticated.']);
	}

	protected static function createDefaultLoadouts(User $user)
	{
		$sword = Weapon::getSword();
		$shield = Weapon::getShield();
		$bow = Weapon::getBow();
		$wand = Weapon::getWand();
		$staff = Weapon::getStaff();

		$swordLoadout = Loadout::firstOrCreate(['user_id' => $user->id, 'weapon_id' => $sword->id]);
		$shieldLoadout = Loadout::firstOrCreate(['user_id' => $user->id, 'weapon_id' => $shield->id]);
		$bowLoadout = Loadout::firstOrCreate(['user_id' => $user->id, 'weapon_id' => $bow->id]);
		$wandLoadout = Loadout::firstOrCreate(['user_id' => $user->id, 'weapon_id' => $wand->id]);
		$staffLoadout = Loadout::firstOrCreate(['user_id' => $user->id, 'weapon_id' => $staff->id]);

		$swordLoadout->side_light_id = Skill::where('weapon_id', $sword->id)
			->where('name', 'Stab')
			->first()
			->id;
		$swordLoadout->neutral_light_id = Skill::where('weapon_id', $sword->id)
			->where('name', 'Swipe')
			->first()
			->id;
		$swordLoadout->save();

		$shieldLoadout->neutral_heavy_id = Skill::where('weapon_id', $shield->id)
			->where('name', 'Block')
			->first()
			->id;
		$shieldLoadout->side_light_id = Skill::where('weapon_id', $shield->id)
			->where('name', 'Bash')
			->first()
			->id;
		$shieldLoadout->save();

		$bowLoadout->side_light_id = Skill::where('weapon_id', $bow->id)
			->where('name', 'Swipe')
			->first()
			->id;
		$bowLoadout->neutral_light_id = Skill::where('weapon_id', $bow->id)
			->where('name', 'SingleShot')
			->first()
			->id;
		$bowLoadout->save();

		$wandLoadout->neutral_light_id = Skill::where('weapon_id', $wand->id)
			->where('name', 'Fireball')
			->first()
			->id;
		$wandLoadout->side_light_id = Skill::where('weapon_id', $wand->id)
			->where('name', 'Swipe')
			->first()
			->id;
		$wandLoadout->save();

		$staffLoadout->side_light_id = Skill::where('weapon_id', $staff->id)
			->where('name', 'Whack')
			->first()
			->id;
		$staffLoadout->neutral_light_id = Skill::where('weapon_id', $staff->id)
			->where('name', 'IceBolt')
			->first()
			->id;
		$staffLoadout->save();

		$user->loadout_primary_id = $swordLoadout->id;
		$user->loadout_secondary_id = $shieldLoadout->id;
		$user->save();
	}
}
