<?php

namespace App\Policies;

use App\Models\Loadout;
use App\Models\User;
use Illuminate\Auth\Access\HandlesAuthorization;
use Illuminate\Auth\Access\Response;

class LoadoutPolicy
{
	use HandlesAuthorization;

	/**
	 * Determine whether the user can view the model.
	 */
	public function view(User $user, Loadout $loadout): Response
	{
		return $this->update($user, $loadout);
	}

	/**
	 * Determine whether the user can update the model.
	 *
	 * @param User $user
	 * @param Loadout $loadout
	 *
	 * @return Response
	 */
	public function update(User $user, Loadout $loadout): Response
	{
		if ($user->id === $loadout->user_id)
		{
			return Response::allow();
		}

		return Response::denyAsNotFound();
	}
}
