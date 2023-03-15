<?php

use App\Http\Controllers\AuthController;
use App\Http\Controllers\LoadoutController;
use App\Http\Controllers\SkillController;
use App\Http\Controllers\UserController;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::middleware('auth:sanctum')->get('/user', function(Request $request)
{
	return $request->user();
});

// Auth
Route::post('/register', [AuthController::class, 'register'])
	->name('register');

Route::post('/login', [AuthController::class, 'login'])
	->name('login');

Route::get('/whoami', [AuthController::class, 'whoami'])
	->name('whoami');

// Users
Route::resource('users', UserController::class)->only([
	'index',
	'show',
	'update',
	'delete',
]);

// Skills
Route::get('/skills', [SkillController::class, 'index']);
Route::controller(SkillController::class)->group(function() {
	Route::get('/skills/{skill}', 'show');
	Route::post('/skills/{skill}/unlock', 'unlock');
});

// Loadouts
Route::controller(LoadoutController::class)->group(function() {
	Route::get('/loadouts', 'index');
	Route::post('/loadouts/swap', 'swap')->name('loadouts.swap');
	Route::get('/loadouts/{loadout}', 'show');
	Route::post('/loadouts/{loadout}/primary', 'primary');
	Route::post('/loadouts/{loadout}/secondary', 'secondary');
	Route::post('/loadouts/{loadout}/equip/{skill}', 'equip');
});
