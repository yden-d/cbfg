<?php

namespace App\Http\Controllers;

use App\Models\Skill;
use App\Models\User;
use Illuminate\Auth\AuthenticationException;
use Illuminate\Database\Eloquent\Collection;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\Auth;

class SkillController extends Controller
{
	/**
	 * Get the skills for the authenticated user.
	 */
	public function index(): Collection
	{
		return Skill::all();
	}

	public function show(Skill $skill): Skill
	{
		return $skill;
	}

	/**
	 * Attempt to unlock a skill for the authenticated user.
	 *
	 * @param Skill $skill
	 *
	 * @return JsonResponse|void
	 * @throws AuthenticationException
	 */
	public function unlock(Skill $skill)
	{
		if (!Auth::check())
		{
			throw new AuthenticationException;
		}
		/** @var User $user */
		$user = Auth::user();
		if ($user->hasSkill($skill))
		{
			return response()->json(['message' => "$user->username already has $skill->name."]);
		}
		if (!$user->canUnlock($skill))
		{
			return response()->json(['message' => "$user->username cannot unlock $skill->name."], 400);
		}
		$user->addSkill($skill);
	}
}