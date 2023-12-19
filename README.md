EQBotEquipment

A little utility to equip bots quickly using the included equipment profiles in Data.xml, or .Bot files from GeorgeS' EQItems editor.

Edit the included Data.xml file and update the <Database> record with your db credentials.  Save and run the executable.

If you have existing .bot files from EQItems and want to use them, put them in a 'profiles' folder alongside the executable.  

The app will attempt to import them into the Data.xml (and then you can remove them) and assign the correct class based on the filename.

Examples: SHAMAN_UBER.bot, shm_newb.bot will both be assigned to the Shaman class.

Duplicate profiles are determined by the class id and the item list and are not imported, so it will not import the same set multiple times.

Alternate names in the filename for classes (necro, nec, necromancer) is handled and can be achieved by editing the Data.xml and updating the ALT attribute for the appropriate class.

See Warrior or Magician for examples.

If a class is not able to be determined upon import, '-1' will be used and it will be available to all bots in the menus.
