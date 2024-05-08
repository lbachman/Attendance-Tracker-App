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