# TimerRccg - Timer Application with Web Remote Control

A Windows Forms timer application with schedule management and web-based remote control capabilities.

## Features

- **Timer Management**: Set custom timers with titles and durations
- **Schedule Management**: Create and manage multiple timer schedules
- **Multi-Screen Support**: Display timer on secondary screens
- **Web Remote Control**: Control the timer remotely via web browser on any device

## Web Remote Control Setup

The application includes a built-in web server that allows you to control the timer from any device on your local network (phone, tablet, laptop, etc.).

### First-Time Setup

1. **Run as Administrator** (required only once):
   - Right-click `TimerRccg.exe` → "Run as administrator"
   - This automatically configures URL permissions for network access

2. **Configure Windows Firewall** (run in Command Prompt as Administrator):
   ```cmd
   netsh advfirewall firewall add rule name="TimerRccg 8080 Any" dir=in action=allow protocol=TCP localport=8080 profile=any
   ```

3. **Set Network Profile to Private**:
   - Go to Settings → Network & Internet → Wi-Fi
   - Click on your connected network
   - Set Network profile to "Private" (not "Public")

### Using Web Remote Control

1. **Start the application** normally (no longer need to run as admin)
2. **Find your PC's IP address**:
   - Press Win+R → type `cmd` → run `ipconfig`
   - Note the IPv4 Address (e.g., 192.168.1.100)
3. **Access from any device** on the same Wi-Fi network:
   - Open browser and go to: `http://YOUR_PC_IP:8080`
   - Example: `http://192.168.1.100:8080`

### Web Interface Features

- **Live Timer Display**: Shows current timer countdown and title
- **Queue Management**: View all scheduled items with current position highlighted
- **Remote Controls**: Previous/Next buttons to navigate through schedule
- **Auto-Refresh**: Updates every 2 seconds automatically
- **Responsive Design**: Works on phones, tablets, and desktop browsers

### Troubleshooting

**Can't access from phone/other devices:**
- Ensure both devices are on the same Wi-Fi network (not guest network)
- Verify Windows Firewall rule was added successfully
- Check that network profile is set to "Private"
- Disable VPNs temporarily to test
- Some routers have "client isolation" - try a different Wi-Fi network or phone hotspot

**Port 8080 already in use:**
- Close any other applications using port 8080
- Restart the TimerRccg application

**Still having issues:**
- Run `netstat -ano | findstr :8080` to verify the server is listening on 0.0.0.0:8080
- Check `netsh http show urlacl` to verify URL permissions are set

## System Requirements

- Windows 10/11
- .NET Framework 4.7.2 or later
- Administrator privileges (for initial setup only)

## Usage

1. **Set Timer**: Enter title and minutes, click "Set Time"
2. **Create Schedule**: Click "Schedule" to add multiple timer items
3. **Start Program**: Click "Start" to begin the scheduled sequence
4. **Navigate**: Use Previous/Next buttons or web interface to control playback
5. **Remote Control**: Access `http://YOUR_PC_IP:8080` from any device on your network

## Technical Details

- **Web Server**: Built-in HttpListener on port 8080
- **Thread Safety**: All web requests are marshaled to UI thread
- **REST API**: 
  - `GET /` - Web dashboard
  - `GET /status` - JSON status data
  - `POST /next` - Move to next item
  - `POST /previous` - Move to previous item
