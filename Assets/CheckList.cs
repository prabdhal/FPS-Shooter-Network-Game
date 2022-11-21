/*
 * 
 * COMPLETED
 * == Weapon 
 * Raycasting functionality 
 * > when holding the Fire button, a raycast will be active and trigger damage depending on weapon "Fire rate"
 * Create crosshair for aim and fire
 * Add Reload functionality 
 * Add Team colours and option to change teams via buttons
 * Add UI to show player is reloading
 * Add friendly fire functionality so that same team members cannot damage eachother
 * Show global chat where it shows who killed and died (has something to do with not recognizing ref from weapon script...)
 * 
 * BUG FIXES
 * When client joins... they don't see the color changes applied (WAS GETTING NULL OBJECT REF TO PLAYERHUD... BLOCKED OTHER CODE) 
 * 
 * 
 *
 *
 * TO DO
 * BUGS
 * When killing client, global kill logs are double instantiated
 * 
 * == Game
 * Add death and respawn with screen turning grey and spawn timer countdown on screen (hide player, apply reset, teleport to another spawn point)
 * When client joins they don't see the global chat list
 * player name isnt reflected across clients
 * 
 * 
 * == Player HUD
 * Client players need to access other client player canvas's and set the canvas to LookAt client camera
 * Modify raycast get component portion to be more efficient
 * 
 * == Weapon
 * Add Pick up weapon functionality
 * Update automatic weapon click fire vs general fire rate (faster to click and fire...)
 * Add aim down sight for some weapons like ARs, Sniper rifles
 * Add weapon accuracy with random points around raycast as bullets, the further the range... the worse the accuracy (larger circle)
 * Ex.
 *   Vector2 spreadDirection = Random.insideUnitCircle.normalized; //Get a random direction for the spread
     Vector3 offsetDirection = new Vector3(fpsCam.transform.right.x * spreadDirection.x, fpsCam.transform.up * spreadDirection.y, 0); //Align direction with fps cam direction
 
     float offsetMagnitude = Random.Range(0f, maxSpreadAmount); //Get a random offset amount
     offsetMagnitude = Mathf.Tan(offsetMagnitude); //Convert to segment length so we get desired degrees value
     Vector3 bulletTrajectory = fpsCam.transform.forward + (offsetDirection * offsetMagnitude); //Add our offset to our forward vector
 
     RaycastHit hit;
     if (Physics.Raycast(fpsCam.transform.position, bulletTrajectory, out hit, range))
     {
     }
 * 
 * Add functionality for burst shot
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */