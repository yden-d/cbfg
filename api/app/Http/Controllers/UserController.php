<?php

namespace App\Http\Controllers;

use App\Models\User;
use Illuminate\Database\Eloquent\Collection;
use Illuminate\Http\Request;

class UserController extends Controller
{
	/**
	 * Display a listing of the resource.
	 *
	 */
	public function index(): Collection
	{
		return User::all();
	}

	/**
	 * Display the specified resource.
	 *
	 * @param \App\Models\User $user
	 *
	 * @return \Illuminate\Http\Response
	 */
	public function show(User $user)
	{
		//
	}

	/**
	 * Update the specified resource in storage.
	 *
	 * @param \Illuminate\Http\Request $request
	 * @param \App\Models\User $user
	 *
	 * @return \Illuminate\Http\Response
	 */
	public function update(Request $request, User $user)
	{
		//
	}

	/**
	 * Remove the specified resource from storage.
	 *
	 * @param \App\Models\User $user
	 *
	 * @return \Illuminate\Http\Response
	 */
	public function destroy(User $user)
	{
		//
	}
}
