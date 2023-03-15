<?php

namespace App\Http\Controllers;

use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use App\Models\Loadout;
use App\Models\Skill;
use App\Models\User;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Gate;

class LoadoutController extends Controller
{
	public function __construct()
	{
		$this->authorizeResource(Loadout::class, 'loadout');
	}

	public function index()
	{
		return Loadout::all();
	}

	public function show(Loadout $loadout): Loadout
	{
		return $loadout;
	}

	public function equip(Loadout $loadout, Skill $skill): JsonResponse
	{
		// Forbid updating skills not belonging to the user.
		$this->authorize('update', $loadout);
		/** @var User */
		$user = Auth::user();
		// Forbid equipping skills that are locked to the user.
		if (!$user->hasSkill($skill))
		{
			return response()->json([
				'message' => "$user->username has not unlocked $skill->name.",
			], 403);
		}
		// Forbid equipping skills from another weapon.
		if ($skill->weapon_id !== $loadout->weapon_id)
		{
			return response()->json([
				'message' => "Cannot equip {$skill->weapon->name} skill to {$loadout->weapon->name} loadout."
			], 400);
		}
		// Set the loadout's binding (as determined by the skill's binding) to the skill's ID.
		$loadout->{str_replace(' ', '_', strtolower($skill->binding)) . '_id'} = $skill->id;
		$loadout->save();
	}

	public function primary(Loadout $loadout)
	{
		// Forbid equipping loadouts not belonging to the user.
		$this->authorize('update', $loadout);
		/** @var User */
		$user = Auth::user();
		// If loadout is equipped as secondary, swap the two.
		if ($loadout->id == $user->loadout_secondary_id)
		{
			$this->swap();
		}
		$user->loadout_primary_id = $loadout->id;
		$user->save();
	}

	public function secondary(Loadout $loadout)
	{
		// Forbid equipping loadouts not belonging to the user.
		$this->authorize('update', $loadout);
		/** @var User */
		$user = Auth::user();
		// If loadout is equipped as primary, swap the two.
		if ($loadout->id == $user->loadout_primary_id)
		{
			$this->swap();
		}
		$user->loadout_secondary_id = $loadout->id;
		$user->save();
	}

	/**
	 * Swap the authenticated user's primary and secondary loadouts.
	 */
	public function swap()
	{
		$user = Auth::user();
		$id = $user->loadout_primary_id;
		$user->loadout_primary_id = $user->loadout_secondary_id;
		$user->loadout_secondary_id = $id;
		$user->save();
	}
}
