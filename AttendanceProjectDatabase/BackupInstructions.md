# Backup instructions for linux (take with a grain of salt I barely know anything about linux and implementation on server will probably require troubleshooting).
1. Basic script for backups in bash (I used rclone to send backups to my OneDrive which can be installed on the website. The reason I say this is because I have no idea if that will be allowed to be installed on the server or if it is already installed. Setup is easy all you have to do is run the following command ```rclone config``` and follow the prompts):
## Script for OneDrive (file included in folder):
```
#!/bin/bash

# MySQL database credentials
DB_USER="your_username"
DB_PASSWORD="your_password"
DB_NAME="your_database"

# Backup directory
BACKUP_DIR="/path/to/backup"

# OneDrive directory
ONEDRIVE_DIR="remote:backups"

# Timestamp (for backup file name)
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

# Backup file name
BACKUP_FILE="$BACKUP_DIR/$DB_NAME-$TIMESTAMP.sql"

# Create backup directory if it doesn't exist
mkdir -p $BACKUP_DIR

# Backup MySQL database
mysqldump -u $DB_USER -p$DB_PASSWORD $DB_NAME > $BACKUP_FILE

# Check if backup was successful
if [ $? -eq 0 ]; then
    echo "Backup of $DB_NAME completed successfully. Backup file: $BACKUP_FILE"
    
    # Sync backup file to OneDrive
    rclone copy $BACKUP_FILE $ONEDRIVE_DIR
    if [ $? -eq 0 ]; then
        echo "Backup file synced to OneDrive successfully."
    else
        echo "Failed to sync backup file to OneDrive."
    fi
else
    echo "Backup of $DB_NAME failed."
fi
```
---
## Script without Onedrive (file included in folder): 
```
#!/bin/bash

# MySQL database credentials
DB_USER="your_username"
DB_PASSWORD="your_password"
DB_NAME="your_database"

# Backup directory
BACKUP_DIR="/path/to/backup"

# Timestamp (for backup file name)
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

# Backup file name
BACKUP_FILE="$BACKUP_DIR/$DB_NAME-$TIMESTAMP.sql"

# Create backup directory if it doesn't exist
mkdir -p $BACKUP_DIR

# Backup MySQL database
mysqldump -u $DB_USER -p$DB_PASSWORD $DB_NAME > $BACKUP_FILE

# Check if backup was successful
if [ $? -eq 0 ]; then
    echo "Backup of $DB_NAME completed successfully. Backup file: $BACKUP_FILE"
else
    echo "Backup of $DB_NAME failed."
fi
```
---
2. Test script in terminal: 
```
./mysql_backup(OD or NOOD).sh
```
---
3. Utilize cron jobs by running this in terminmal:
```
crontab -e
```
---
4. Add a new line to schedule the back script to run daily. For example, to run the backup script every day at midnight, you can add the following line (replace the /path/to/ with the actual path and OD or NOOD with the corresponding script used):
```
0 0 * * * /path/to/mysql_backup(OD or NOOD).sh
```
---
5. Save the exit the editor. It should now be scheduled to run daily.
---
6. For manual add a button to run this script from the desktop app.
