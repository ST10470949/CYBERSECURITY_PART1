using System;
using System.IO;
using System.Media;
using System.Threading;

// Chatbot helper class containing core chat logic and utilities.
static class Chatbot
{
    // Simple in-memory conversation memory (per run)
    // Stores the user's name, mood, and a small list of preferences.
    private static string memoryName = null;
    private static string memoryMood = null;
    private static System.Collections.Generic.List<string> memoryPreferences = new System.Collections.Generic.List<string>();

    // ProcessMemoryInput looks for phrases that set or request remembered values.
    // Returns a reply when a memory action is handled, otherwise null.
    public static string ProcessMemoryInput(string rawInput, string lower)
    {
        if (string.IsNullOrWhiteSpace(rawInput)) return null;

        // Set name
        string[] namePrefixes = new[] { "my name is ", "i am ", "i'm ", "call me " };
        foreach (var p in namePrefixes)
        {
            int idx = lower.IndexOf(p, StringComparison.Ordinal);
            if (idx == 0)
            {
                string value = rawInput.Substring(p.Length).Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    memoryName = value;
                    // also update shared user store so other parts of app use the name
                    UserStore.CurrentUserName = memoryName;
                    return $"Nice to meet you, {memoryName}. I'll remember your name.";
                }
            }
        }

        // Ask for name
        if (lower.Contains("what is my name") || lower.Contains("what's my name") || lower.Contains("do you know my name"))
        {
            if (!string.IsNullOrEmpty(memoryName)) return $"Your name is {memoryName}.";
            if (!string.IsNullOrEmpty(UserStore.CurrentUserName)) return $"Your name is {UserStore.CurrentUserName}.";
            return "I don't know your name yet. You can tell me by saying 'my name is ...'";
        }

        // Set mood
        string[] moodPrefixes = new[] { "my mood is ", "i feel ", "i'm feeling ", "i am feeling " };
        foreach (var p in moodPrefixes)
        {
            int idx = lower.IndexOf(p, StringComparison.Ordinal);
            if (idx == 0)
            {
                string value = rawInput.Substring(p.Length).Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    memoryMood = value;
                    return $"Thanks for telling me — I have noted that you're feeling {memoryMood}.";
                }
            }
        }

        // Ask for mood
        if (lower.Contains("what is my mood") || lower.Contains("how am i feeling") || lower.Contains("what am i feeling"))
        {
            if (!string.IsNullOrEmpty(memoryMood)) return $"You told me you're feeling {memoryMood}.";
            return "I don't know how you're feeling yet. You can tell me by saying 'I feel ...'";
        }

        // Preferences: set simple preferences
        string[] prefPrefixes = new[] { "i prefer ", "my preference is ", "i like " };
        foreach (var p in prefPrefixes)
        {
            int idx = lower.IndexOf(p, StringComparison.Ordinal);
            if (idx == 0)
            {
                string value = rawInput.Substring(p.Length).Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    memoryPreferences.Add(value);
                    return $"Noted. I'll remember that you prefer {value}.";
                }
            }
        }

        // Ask for preferences
        if (lower.Contains("what are my preferences") || lower.Contains("what do i prefer") || lower.Contains("what do i like"))
        {
            if (memoryPreferences.Count > 0) return "You told me you prefer: " + string.Join(", ", memoryPreferences) + ".";
            return "You haven't told me any preferences yet. Say 'I prefer ...' to set one.";
        }

        // Forget commands
        if (lower.Contains("forget my name") || lower.Contains("clear my name"))
        {
            memoryName = null;
            return "Okay, I have forgotten your name.";
        }

        if (lower.Contains("forget my mood") || lower.Contains("clear my mood"))
        {
            memoryMood = null;
            return "Okay, I have forgotten your mood.";
        }

        // not a memory-related input
        return null;
    }

    // The Chatbot helper previously handled padding/truncation to an exact
    // word count. Keep the helper minimal with a simple EnsureBodyLength
    // method available if needed elsewhere in the project.
    // EnsureBodyLength pads or truncates a text to a target word count.
    // This is optional helper used by other components if needed.
    public static string EnsureBodyLength(string baseText, int targetWords)
    {
        var words = new System.Collections.Generic.List<string>();
        foreach (var w in System.Text.RegularExpressions.Regex.Split(baseText ?? string.Empty, "\\s+"))
        {
            if (!string.IsNullOrEmpty(w)) words.Add(w);
        }

        string filler = "Following these recommended practices will reduce risk, improve resilience, and help you maintain a safer digital environment over time.";
        var fillerWords = new System.Collections.Generic.List<string>();
        foreach (var w in System.Text.RegularExpressions.Regex.Split(filler, "\\s+"))
        {
            if (!string.IsNullOrEmpty(w)) fillerWords.Add(w);
        }

        while (words.Count < targetWords)
        {
            int need = targetWords - words.Count;
            int take = Math.Min(need, fillerWords.Count);
            for (int i = 0; i < take; i++) words.Add(fillerWords[i]);
            if (fillerWords.Count == 0) break;
        }

        if (words.Count > targetWords) words = words.GetRange(0, targetWords);
        return string.Join(" ", words);
    }

    // Typing animation used across the app. Responses will be printed in cyan.
    // TypeText prints text with a typing animation in cyan color.
    public static void TypeText(string text)
    {
        var previousColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(18);
            }
            Console.WriteLine();
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }

    // Levenshtein distance helper used by fuzzy suggestion logic
    // Compute Levenshtein distance (edit distance) between two strings.
    public static int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) return string.IsNullOrEmpty(b) ? 0 : b.Length;
        if (string.IsNullOrEmpty(b)) return a.Length;

        int[,] d = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) d[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[a.Length, b.Length];
    }

    // Keyword-based quick responses.
    // Scans the input for many common keywords and returns a short reply
    // or null when no keyword is matched. Keeps responses natural and varied.
    public static string GetKeywordResponse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        string lower = input.ToLowerInvariant();

        // greetings
        if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey"))
            return "Hello! How can I help you with cybersecurity today?";

        // thanks
        if (lower.Contains("thank") || lower.Contains("thanks"))
            return "You’re welcome — happy to help. Ask me anything about online safety.";

        // asking name
        if (lower.Contains("your name") || lower.Contains("who are you") || lower.Contains("what are you"))
            return "I am Cyber Times With Abo, your cybersecurity assistant. I can explain threats and give practical advice.";

        // asking purpose or about
        if (lower.Contains("purpose") || lower.Contains("what do you do") || lower.Contains("about you"))
            return "I help people learn about online risks and how to protect themselves — passwords, phishing, malware, privacy and more.";

        // small talk: weather/time/date
        if (lower.Contains("weather"))
            return "I can't check live weather, but remember to avoid clicking unknown links in weather alerts — they can be phishing.";
        if (lower.Contains("time"))
            return "The current time is " + DateTime.Now.ToShortTimeString() + ".";
        if (lower.Contains("date"))
            return "Today's date is " + DateTime.Now.ToShortDateString() + ".";

        // polite goodbyes
        if (lower.Contains("bye") || lower.Contains("goodbye") || lower.Contains("see you"))
            return "Goodbye — stay safe online!";

        // ask for help
        if (lower.Contains("help") || lower.Contains("how do i") || lower.Contains("how to"))
            return "Tell me the topic you need help with, for example 'passwords', 'phishing', or 'safe browsing'.";

        // casual request for a joke
        if (lower.Contains("joke") || lower.Contains("funny"))
            return "Why did the computer get cold? Because it left its Windows open. 😄";

        // default: no keyword matched
        return null;
    }

    // Detect basic sentiment in user input and respond empathetically.
    // Also updates the in-memory mood so future replies can reference it.
    public static string DetectSentimentResponse(string rawInput, string lower)
    {
        if (string.IsNullOrWhiteSpace(rawInput)) return null;

        // sadness
        if (lower.Contains("sad") || lower.Contains("depress") || lower.Contains("unhappy") || lower.Contains("upset"))
        {
            memoryMood = "sad";
            return "I'm sorry you're feeling sad. I'm here for you. If this is about online safety I can help step by step.";
        }

        // happiness / positive
        if (lower.Contains("happy") || lower.Contains("glad") || lower.Contains("awesome") || lower.Contains("great") || lower.Contains("cheerful"))
        {
            memoryMood = "happy";
            return "That's great to hear! I'm glad you're feeling good. If you want, I can share quick tips to keep your accounts safe while you enjoy your day.";
        }

        // anxiety / stress
        if (lower.Contains("anxious") || lower.Contains("stressed") || lower.Contains("worried") || lower.Contains("nervous"))
        {
            memoryMood = "anxious";
            return "I can understand feeling anxious. If something online is worrying you, tell me and I can guide you through the next steps.";
        }

        // anger / frustration
        if (lower.Contains("angry") || lower.Contains("mad") || lower.Contains("frustrat"))
        {
            memoryMood = "angry";
            return "I'm sorry you're feeling angry. Taking a short break can help — when you're ready, I can help with any security issues causing frustration.";
        }

        // If we have a stored mood, respond when user indicates improvement
        if (!string.IsNullOrEmpty(memoryMood))
        {
            if (memoryMood == "sad" && lower.Contains("better")) return "I'm glad you're feeling better than before.";
            if (memoryMood == "anxious" && (lower.Contains("calm") || lower.Contains("calmer"))) return "It's good to hear you're feeling calmer.";
        }

        return null;
    }

    // Display the primary menu used by the main chat loop
    // DisplayMenu shows the main menu to the user in cyan color.
    public static void DisplayMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔════════════════════ MENU ════════════════════╗
║ 1 — How are you?                             ║
║ 2 — What is your purpose?                    ║
║ 3 — Password Safety                          ║
║ 4 — Phishing Awareness                       ║
║ 5 — Safe Browsing                            ║
║ 6 — Malware Information                      ║
║ 7 — Public Wi-Fi Risks                       ║
║ 8 — Exit                                     ║
║ 9 — Extended Topics                          ║
║ 10 — Software Updates                        ║
║ 11 — Encryption                              ║
║ 12 — Firewalls                               ║
║ 13 — Backup & Recovery                       ║
║ 14 — Incident Response                       ║
║ 15 — Privacy Settings                        ║
║ 16 — IoT Security                            ║
║ 17 — Mobile Security                         ║
║ 18 — Secure Coding                           ║
║ 19 — Network Segmentation                    ║
║ 20 — Access Control                          ║
║ 21 — Security Policies                       ║
╚══════════════════════════════════════════════╝
");
        Console.ResetColor();
    }

    // Print an error message in red to the console.
    public static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    // Main chat loop moved to Chatbot so Program remains small
    // StartChat runs the main interactive chat loop for the given user.
    public static void StartChat(string name)
    {
        // ensure the shared user store is populated for helpers
        UserStore.CurrentUserName = name;

        // conversation state used to track simple follow-up flow (per session)
        string convoState = null;

        while (true)
        {
            DisplayMenu();

            Console.Write("\nYour choice: ");
            string input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                ShowError("Please enter a valid option.");
                continue;
            }

            // normalize once for reuse
            string lower = input.ToLower();

            // Conversation flow: handle expected follow-up answers
            if (convoState == "awaiting_howareyou_reply")
            {
                if (lower.Contains("fine") || lower.Contains("good") || lower.Contains("well") || lower.Contains("okay") || lower.Contains("ok"))
                {
                    TypeText("That's great to hear!");
                }
                else if (lower.Contains("not") || lower.Contains("bad") || lower.Contains("sad") || lower.Contains("tired") || lower.Contains("unwell"))
                {
                    TypeText("I'm sorry to hear that. If it's related to online safety I can help. Otherwise, take care and rest.");
                }
                else
                {
                    TypeText("Thanks for telling me. How else can I help?");
                }
                // clear state after handling
                convoState = null;
                continue;
            }

            // Initiate simple follow-up flows
            if (lower.Contains("how are you") || lower.Contains("how r you") || lower.Contains("how are u") || lower == "how are you?")
            {
                TypeText("I'm good! How about you?");
                convoState = "awaiting_howareyou_reply";
                continue;
            }

            // Memory processing: store/recall name, mood, preferences
            var memReply = ProcessMemoryInput(input, lower);
            if (!string.IsNullOrEmpty(memReply))
            {
                TypeText(memReply);
                continue;
            }

            // Sentiment detection and empathetic response
            var sentimentReply = DetectSentimentResponse(input, lower);
            if (!string.IsNullOrEmpty(sentimentReply))
            {
                TypeText(sentimentReply);
                continue;
            }

            // Quick keyword responses — check simple keywords before heavier processing
            var quick = GetKeywordResponse(input);
            if (!string.IsNullOrEmpty(quick))
            {
                TypeText(quick);
                continue;
            }

            // load the shared topic map used for menu and free-form queries
            var menuTopics = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
            {
                { "password", "Strong password security is one of the most critical aspects of protecting your digital identity in today’s interconnected world. A secure password should be long, ideally between 12 and 16 characters or more, and include a combination of uppercase letters, lowercase letters, numbers, and special symbols. This complexity makes it significantly harder for attackers to crack passwords using brute-force or dictionary attacks. Avoid using personal information such as your name, surname, birthdate, or common words, as these are easy for attackers to guess using social engineering or automated tools. Each account should have a unique password, because reusing passwords across multiple platforms increases the risk of widespread compromise if one account is breached. Password managers are highly recommended because they can generate strong passwords and store them securely. Additionally, regularly updating passwords and monitoring account activity helps detect unauthorized access early. Combining strong passwords with two-factor authentication adds an extra layer of security, making it far more difficult for attackers to gain access." },

                { "phishing", "Phishing is a deceptive cyberattack method used by criminals to trick individuals into revealing sensitive information such as usernames, passwords, banking details, or personal identification data. These attacks often appear as legitimate communications from trusted organizations like banks, government agencies, or popular online platforms. Phishing messages are typically delivered via email, SMS, or fake websites that closely resemble real ones. Attackers frequently create a sense of urgency, such as warning about account suspension or suspicious activity, to pressure victims into acting quickly without verifying the authenticity of the request. Common warning signs include poor grammar, unfamiliar sender addresses, suspicious links, and unexpected attachments. To protect yourself, always verify the sender’s identity and avoid clicking on links from unknown or untrusted sources. Instead, manually type the official website address into your browser. Using spam filters and security tools can help detect phishing attempts. Ultimately, awareness and cautious behavior are the most effective defenses against phishing attacks." },

                { "malware", "Malware refers to malicious software designed to harm, exploit, or gain unauthorized access to computer systems and networks. Common types include viruses, worms, trojans, spyware, ransomware, and adware. Malware can infect devices through various methods, such as downloading files from untrusted sources, opening infected email attachments, clicking malicious links, or using compromised removable media. Once installed, malware can perform harmful activities like stealing sensitive information, monitoring user behavior, corrupting files, slowing system performance, or granting attackers remote control of the device. Some malware operates silently, making it difficult to detect without specialized tools. To defend against malware, users should install reputable antivirus and anti-malware software and keep it updated regularly. Avoid downloading software from unknown websites and always scan files before opening them. Keeping your operating system and applications updated is essential because updates patch vulnerabilities that malware often exploits. Practicing safe browsing habits and being cautious with email attachments significantly reduces the risk of infection." },

                { "social engineering", "Social engineering is a cyberattack technique that manipulates individuals into revealing confidential information or performing actions that compromise security. Instead of targeting technical vulnerabilities, attackers exploit human psychology, using tactics such as fear, urgency, curiosity, or authority to influence behavior. For example, an attacker may impersonate a bank official, IT technician, or colleague to gain trust and request sensitive information like passwords or financial details. These attacks can occur through emails, phone calls, text messages, or even in person. Social engineering is particularly dangerous because it relies on human error rather than system weaknesses. To protect yourself, always verify the identity of anyone requesting sensitive information, especially if the request seems urgent or unusual. Never share passwords or confidential data over unsecured channels. Organizations typically have policies that prevent employees from asking for such information directly. Awareness, skepticism, and proper training are essential in preventing social engineering attacks and protecting sensitive information." },

                { "2fa", "Two-Factor Authentication (2FA) is a security measure that adds an additional layer of protection to online accounts by requiring two forms of verification. Instead of relying solely on a password, 2FA requires a second form of verification, such as a one-time code sent to your phone, a code generated by an authentication app, or biometric verification like a fingerprint or facial recognition. This ensures that even if a password is compromised, unauthorized users cannot access the account without the second factor. 2FA is especially important for sensitive accounts such as email, banking, and social media platforms. Authentication apps are generally more secure than SMS-based codes, as they are less vulnerable to interception or SIM-swapping attacks. Enabling 2FA significantly reduces the risk of unauthorized access and is considered a best practice in cybersecurity. Many online services now offer 2FA, and users are strongly encouraged to enable it wherever possible for enhanced protection." },

                { "safe browsing", "Safe browsing involves adopting practices that protect users from online threats while navigating the internet. One key aspect is ensuring that websites use HTTPS, which indicates a secure, encrypted connection between the user and the website. Users should avoid downloading files or software from unknown or untrusted sources, as these may contain malware. Pop-ups that claim your device is infected or urge immediate action are often scams and should be ignored. Modern web browsers include built-in security features that help detect and block malicious websites, making it important to keep browsers updated. Additionally, browser extensions that block ads, trackers, and harmful scripts can enhance security. Users should also be cautious when entering personal information online and ensure they are on legitimate websites. Practicing safe browsing habits reduces the risk of cyberattacks, protects personal data, and ensures a safer online experience overall." },

                { "ransomware", "Ransomware is a type of malicious software that encrypts a victim’s files and demands payment, typically in cryptocurrency, to restore access. Once infected, users are often locked out of their systems and presented with a ransom note containing payment instructions. However, paying the ransom does not guarantee that access will be restored and may encourage further criminal activity. Ransomware commonly spreads through phishing emails, malicious attachments, or compromised websites. The best defense against ransomware is prevention, including regularly backing up important data using the 3-2-1 rule and storing backups securely offline or in the cloud. Users should avoid opening suspicious emails or attachments and ensure their security software and operating systems are up to date. If an infection occurs, the affected device should be disconnected from the network immediately to prevent the spread. Strong cybersecurity practices significantly reduce the risk of ransomware attacks." },

                { "public wifi", "Public Wi-Fi networks, such as those in cafes or airports, are convenient but often insecure. Attackers can intercept unencrypted traffic and capture credentials or personal information. Avoid accessing sensitive accounts while connected to public Wi-Fi unless you use a Virtual Private Network (VPN), which encrypts your internet traffic. Also, disable automatic connection to open networks and ensure file sharing is turned off when using public connections." },

                { "identity theft", "Identity theft occurs when criminals steal personal information and use it to commit fraud, such as opening bank accounts or making purchases in your name. Information can be obtained through phishing, data breaches, or social media oversharing. Protect yourself by limiting the personal details you share online, monitoring financial statements regularly, and using credit alerts if available. If identity theft occurs, report it immediately to your bank and relevant authorities to minimize damage." },

                { "updates", "Software updates are essential for security because they patch vulnerabilities that attackers exploit. Outdated systems are one of the most common entry points for cybercriminals. Updates often include fixes for newly discovered security flaws, performance improvements, and new protection features. Enable automatic updates whenever possible for your operating system, applications, and antivirus software to ensure you remain protected without needing manual intervention." },

                { "encryption", "Encryption converts data into a coded format to prevent unauthorized access. Strong encryption protects data at rest and in transit. Use well-known standards (e.g., TLS for web traffic, AES for storage) and ensure keys are managed securely. Encryption is one layer of defense but must be combined with access controls and key management for full effectiveness." },

                { "firewalls", "Firewalls monitor and control incoming and outgoing network traffic based on predetermined security rules. They can be host-based or network appliances. Use firewalls to segment networks, restrict unnecessary ports/services, and log suspicious activity. Regularly review rules to keep them minimal and relevant." },

                { "backup & recovery", "Reliable backups protect against data loss from ransomware, hardware failure, or human error. Follow the 3-2-1 rule: keep three copies, on two media types, with one offsite or immutable. Test recovery procedures regularly so backups can be trusted when needed." },

                { "incident response", "Incident response is a planned set of procedures to detect, contain, eradicate, and recover from security incidents. A good plan defines roles, communication channels, evidence preservation steps, and post-incident review to improve defenses." },

                { "privacy settings", "Privacy settings help control personal data exposure in applications and social media. Review permissions, limit location sharing, disable unnecessary data collection, and prefer privacy-friendly defaults to reduce risk." },

                { "iot security", "Internet of Things (IoT) devices often have limited security. Secure IoT by changing default credentials, applying updates, isolating devices on separate networks, and disabling unused features. Monitor device behavior for anomalies." },

                { "mobile security", "Mobile security includes keeping OS and apps updated, avoiding untrusted app stores, using screen lock and encryption, and being cautious with permissions. Use mobile device management for organization-owned devices." },

                { "secure coding", "Secure coding practices reduce vulnerabilities in software. Validate input, use parameterized queries to prevent injections, apply principle of least privilege, and perform code reviews and static analysis." },

                { "network segmentation", "Network segmentation divides a network into smaller zones to limit lateral movement by attackers. Use VLANs, access controls, and firewall rules to restrict traffic between segments based on need-to-know." },

                { "access control", "Access control ensures users and systems have only the permissions they require. Implement role-based or attribute-based access control, strong authentication, and periodic access reviews." },

                { "security policies", "Security policies document acceptable use, incident response, password policies, and other governance needed to maintain an organization's security posture. Keep policies clear, enforced, and regularly reviewed." }
            };

            switch (input)
            {
                case "1":
                    TypeText($"{UserStore.CurrentUserName}, I am operating optimally and ready to assist you. I can provide detailed guidance on cybersecurity topics, explain risks, and suggest practical steps to protect your online accounts and devices. Ask me about passwords, phishing, malware, privacy practices, safe browsing, or how to respond to suspicious activity, and I will give clear, actionable advice.");
                    break;

                case "2":
                    TypeText($"{UserStore.CurrentUserName}, My purpose is to educate users about digital threats and promote safe online behaviour. I provide explanations of common attack types, best practices to reduce risk, and step-by-step recommendations you can apply immediately. Whether you need help selecting a password manager, recognizing scam emails, or hardening your personal devices, I aim to make cybersecurity understandable and practical.");
                    break;

                case "3":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["password"]);
                    break;

                case "4":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["phishing"]);
                    break;

                case "5":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["safe browsing"]);
                    break;

                case "6":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["malware"]);
                    break;

                case "7":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["public wifi"]);
                    break;

                case "10":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["updates"]);
                    break;

                case "11":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["encryption"]);
                    break;

                case "12":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["firewalls"]);
                    break;

                case "13":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["backup & recovery"]);
                    break;

                case "14":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["incident response"]);
                    break;

                case "15":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["privacy settings"]);
                    break;

                case "16":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["iot security"]);
                    break;

                case "17":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["mobile security"]);
                    break;

                case "18":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["secure coding"]);
                    break;

                case "19":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["network segmentation"]);
                    break;

                case "20":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["access control"]);
                    break;

                case "21":
                    TypeText($"{UserStore.CurrentUserName}, " + menuTopics["security policies"]);
                    break;

                case "9":
                    // Launch external chatbot integration
                    ExternalChatbot.Run();
                    break;

                case "8":
                    // Exit
                    Program.ExitProgram(UserStore.CurrentUserName);
                    return;

                default:
                    // Free-form handling and fuzzy matching (omitted here for brevity)
                    // We'll reuse the same logic as the original implementation by
                    // delegating to the external helper when appropriate or using
                    // the local topicsCore map for natural language handling.
                    {
                        lower = input.ToLower();

                        if (lower.Contains("what can i ask") || lower.Contains("what can i ask you") || lower.Contains("what can i ask about") || lower.Contains("what can you answer"))
                        {
                            TypeText($"{UserStore.CurrentUserName}, You can ask me about: passwords, phishing, malware, social engineering, two-factor authentication (2FA), safe browsing, ransomware, public Wi-Fi safety, identity theft, software updates, encryption, firewalls, backup & recovery, incident response, privacy settings, IoT security, mobile security, secure coding, network segmentation, access control, and security policies.");
                            break;
                        }

                        var topicsCore = menuTopics; // reuse same map for simplicity

                        // tokenize
                        var tokens = new System.Collections.Generic.List<string>();
                        foreach (var t in System.Text.RegularExpressions.Regex.Split(lower, "[^a-z0-9]+"))
                            if (!string.IsNullOrEmpty(t)) tokens.Add(t);

                        var matched = new System.Collections.Generic.List<string>();
                        foreach (var kv in topicsCore)
                        {
                            if (lower.Contains(kv.Key)) matched.Add(kv.Key);
                            else
                            {
                                var keyTokens = kv.Key.Split(' ');
                                foreach (var kt in keyTokens)
                                {
                                    if (tokens.Contains(kt)) { matched.Add(kv.Key); break; }
                                }
                            }
                        }

                        if (matched.Count > 0)
                        {
                            foreach (var key in matched) { TypeText($"{UserStore.CurrentUserName}, {topicsCore[key]}"); Console.WriteLine(); }
                        }
                        else
                        {
                            var suggestions = new System.Collections.Generic.List<string>();
                            foreach (var kv in topicsCore)
                            {
                                var keyTokens = kv.Key.Split(' ');
                                foreach (var kt in keyTokens)
                                foreach (var it in tokens)
                                {
                                    int dist = LevenshteinDistance(kt, it);
                                    int threshold = Math.Max(1, kt.Length / 3);
                                    if (dist <= threshold && !suggestions.Contains(kv.Key)) suggestions.Add(kv.Key);
                                }
                            }

                            if (suggestions.Count > 0)
                            {
                                bool handled = false;
                                foreach (var s in suggestions)
                                {
                                    TypeText($"I think you meant \"{s}\". Please type 'yes' or 'no':");
                                    Console.Write("Your choice: ");
                                    string reply = Console.ReadLine()?.Trim().ToLower();
                                    if (!string.IsNullOrEmpty(reply) && (reply.StartsWith("y") || reply == "yes"))
                                    {
                                        TypeText($"{UserStore.CurrentUserName}, {topicsCore[s]}");
                                        Console.WriteLine();
                                        handled = true;
                                        break;
                                    }
                                }

                                if (!handled) TypeText("Sorry, I can't answer that at the moment. I am still updating and will provide that level of information soon.");
                            }
                            else
                            {
                                TypeText("Sorry, I can't answer that at the moment. I am still updating and will provide that level of information soon.");
                            }
                        }
                    }
                    break;
            }
        }
    }
}

// External chatbot helper previously embedded in Program.cs
static class ExternalChatbot
{
    public static void Run()
    {
        Console.WriteLine("=== Cybersecurity Awareness Chatbot ===");
        Console.WriteLine("Type a topic number or keyword. Type 'exit' to quit.\n");

        while (true)
        {
            ShowMenuForExternal();

            Console.Write("\nYour choice: ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("⚠️ You entered nothing. Please type a number or topic.");
                continue;
            }

            if (input == "exit")
            {
                Console.WriteLine("Stay safe online. Returning to main menu...");
                break;
            }

            if (input == "0" || input == "back")
            {
                Console.WriteLine("Returning to main menu...");
                break;
            }

            string response = GetResponseForExternal(input);
            Chatbot.TypeText(response);
            Console.WriteLine("\n----------------------------------------\n");
        }
    }

    static void ShowMenuForExternal()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔════════════════════ MENU ════════════════════╗
║ 0 — Return to Main Menu                      ║
║ 1 — Password Safety                          ║
║ 2 — Phishing                                 ║
║ 3 — Malware                                  ║
║ 4 — Social Engineering                       ║
║ 5 — Two-Factor Authentication                ║
║ 6 — Safe Browsing                            ║
║ 7 — Ransomware                               ║
║ 8 — Public Wi-Fi Safety                      ║
║ 9 — Identity Theft                           ║
║ 10 — Software Updates                        ║
║ 11 — Encryption                              ║
║ 12 — Firewalls                               ║
║ 13 — Backup & Recovery                       ║
║ 14 — Incident Response                       ║
║ 15 — Privacy Settings                        ║
║ 16 — IoT Security                            ║
║ 17 — Mobile Security                         ║
║ 18 — Secure Coding                           ║
║ 19 — Network Segmentation                    ║
║ 20 — Access Control                          ║
║ 21 — Security Policies                       ║
╚══════════════════════════════════════════════╝
");
        Console.ResetColor();
    }

    static string GetResponseForExternal(string input)
    {
        var topics = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
        {
            { "password", "Strong password security is one of the most important ways to protect your online accounts and digital identity. A secure password should be long, ideally more than 12 characters, and include a mix of uppercase letters, lowercase letters, numbers, and special symbols. Avoid using personal information such as your name, surname, birthdate, or simple patterns like '123456' or 'password', as attackers can easily guess or crack these using automated tools. It is also important to ensure that each account has a unique password so that if one account is compromised, others remain secure. Using a password manager is highly recommended because it can generate complex passwords and store them securely without you needing to remember each one. Additionally, regularly updating your passwords and enabling extra security measures such as two-factor authentication further strengthens your overall protection." },
            { "phishing", "Phishing is a common cyberattack technique where criminals impersonate legitimate organizations or trusted individuals to trick users into revealing sensitive information such as passwords, banking details, or personal identification data. These attacks are often delivered through emails, SMS messages, or fake websites that closely resemble real ones, making them difficult to detect. Attackers typically create a sense of urgency, such as claiming your account will be suspended or that suspicious activity has been detected, to pressure you into acting quickly without thinking. Warning signs include poor spelling, unfamiliar sender addresses, suspicious links, and unexpected attachments. To stay safe, always verify the sender’s identity and avoid clicking on unknown links. Instead, manually type the official website address into your browser. Being cautious and aware of these tactics can significantly reduce your chances of falling victim to phishing scams." },
            { "malware", "Malware refers to any type of malicious software designed to harm, exploit, or gain unauthorized access to a computer system or network. Common types of malware include viruses, worms, trojans, spyware, and ransomware. Malware can infect devices through various methods, such as downloading files from untrusted websites, opening infected email attachments, or clicking on malicious links. Once installed, malware can perform harmful actions like stealing sensitive data, slowing down system performance, corrupting files, or even giving attackers remote control over your device. To protect yourself, it is essential to use reliable antivirus and anti-malware software and keep it updated regularly. Avoid downloading software from unknown sources and always scan files before opening them. Keeping your operating system and applications updated also helps close security vulnerabilities that malware can exploit." },
            { "social engineering", "Social engineering is a type of cyberattack that focuses on manipulating human behavior rather than exploiting technical vulnerabilities. Attackers use psychological tactics to trick individuals into revealing confidential information or performing actions that compromise security. They may impersonate trusted figures such as bank representatives, IT support staff, or colleagues to gain your trust. These attacks often rely on emotions like fear, urgency, curiosity, or authority to pressure victims into acting quickly without verifying the situation. For example, an attacker might call pretending to be from your bank and ask for your PIN or password. To protect yourself, always verify the identity of anyone requesting sensitive information and never share passwords or confidential data over phone calls or messages. Organizations usually have strict policies and will not ask for such details directly." },
            { "2fa", "Two-Factor Authentication (2FA) is a critical security measure that adds an extra layer of protection to your online accounts. Instead of relying solely on a password, 2FA requires a second form of verification, such as a one-time code sent to your mobile phone, a code generated by an authentication app, or biometric data like a fingerprint or facial recognition. This means that even if a hacker manages to steal your password, they still cannot access your account without the second verification factor. 2FA significantly reduces the risk of unauthorized access and is especially important for sensitive accounts such as email, online banking, and social media. Many platforms now offer 2FA as an option, and it is strongly recommended to enable it wherever possible. Using authentication apps is generally more secure than SMS-based codes, as they are less vulnerable to interception." },
            { "safe browsing", "Safe browsing involves taking precautions to protect yourself while using the internet. One of the most important practices is ensuring that websites you visit use HTTPS, which indicates that the connection is encrypted and secure. Avoid downloading files or software from unknown or untrusted websites, as these may contain malware. Be cautious of pop-ups that claim your device is infected or that urge you to download security software immediately, as these are often scams. Modern web browsers include built-in security features that help detect and block malicious websites, so keeping your browser updated is essential. You can also use browser extensions that block ads, trackers, and harmful scripts to enhance your safety. Being aware of online threats and practicing caution when clicking links or entering personal information can significantly reduce your risk." },
            { "ransomware", "Ransomware is a dangerous type of malware that encrypts a victim’s files and demands payment, usually in cryptocurrency, in exchange for restoring access. Once the system is infected, users may be locked out of their files and shown a ransom note with instructions on how to pay. However, paying the ransom does not guarantee that the files will be recovered and may encourage further criminal activity. Ransomware often spreads through phishing emails, malicious attachments, or compromised websites. To protect against ransomware, it is essential to regularly back up important data using the 3-2-1 rule and store backups securely offline or in the cloud. Avoid opening suspicious emails or attachments, and keep your security software and operating system updated. If an infection occurs, disconnect the device from the network immediately to prevent the spread to other systems." },
            { "public wifi", "Public Wi-Fi networks, such as those found in cafes, airports, and hotels, are convenient but often lack proper security measures. This makes them a target for cybercriminals who can intercept data transmitted over the network, including passwords, emails, and financial information. Attackers may use techniques like man-in-the-middle attacks to eavesdrop on your online activity. To stay safe, avoid accessing sensitive accounts, such as online banking, while connected to public Wi-Fi. If necessary, use a Virtual Private Network (VPN), which encrypts your internet traffic and protects your data from interception. Additionally, disable automatic connections to open networks and turn off file sharing on your device. Always verify that you are connecting to a legitimate network and not a fake hotspot set up by attackers." },
            { "identity theft", "Identity theft occurs when criminals steal your personal information and use it to commit fraud or other illegal activities. This can include opening bank accounts, applying for loans, or making purchases in your name. Personal information can be obtained through phishing attacks, data breaches, social media oversharing, or even physical theft of documents. The consequences of identity theft can be severe, including financial loss and damage to your credit record. To protect yourself, limit the amount of personal information you share online and regularly monitor your bank statements and credit reports for suspicious activity. Use strong passwords and enable security features such as 2FA. If you suspect identity theft, report it immediately to your bank and relevant authorities to minimize damage and begin the recovery process." },
            { "updates", "Software updates play a critical role in maintaining the security and performance of your devices. Developers regularly release updates to fix bugs, improve functionality, and most importantly, patch security vulnerabilities that attackers can exploit. Using outdated software increases your risk of being targeted by cybercriminals, as known vulnerabilities are often publicly documented and actively exploited. Enabling automatic updates ensures that your operating system, applications, and antivirus software are always up to date without requiring manual effort. Updates may also introduce new features and improve system stability. It is important not to ignore update notifications, especially for security-critical software. Regularly checking for updates and applying them promptly is one of the simplest yet most effective ways to protect your system from cyber threats." },
            { "encryption", "Encryption is a fundamental security technique that converts readable data into an unreadable coded format to prevent unauthorized access. This process ensures that even if data is intercepted, it cannot be understood without the correct decryption key. Encryption is widely used in securing sensitive information such as passwords, financial transactions, and personal data. There are two main types of encryption: symmetric encryption, which uses a single key for both encryption and decryption, and asymmetric encryption, which uses a pair of keys (public and private). Common standards include AES for data storage and TLS for securing data transmitted over the internet. However, encryption alone is not enough; proper key management is essential to ensure that keys are stored securely and not exposed to attackers. When combined with other security controls like access control and authentication, encryption provides a strong layer of defense against data breaches." },
            { "firewalls", "Firewalls are essential security systems that monitor and control incoming and outgoing network traffic based on predefined security rules. They act as a barrier between trusted internal networks and untrusted external networks such as the internet. Firewalls can be hardware-based, software-based, or a combination of both, and they are used to block unauthorized access while allowing legitimate communication. By filtering traffic based on IP addresses, ports, and protocols, firewalls help prevent cyberattacks such as hacking attempts and malware infections. Modern firewalls may also include advanced features like intrusion detection and prevention systems. It is important to regularly review and update firewall rules to ensure they remain effective and relevant. Poorly configured firewalls can create vulnerabilities instead of protecting against them. When used alongside other security measures, firewalls play a crucial role in maintaining a secure network environment." },
            { "backup & recovery", "Backup and recovery are critical components of any effective data protection strategy. Backups ensure that copies of important data are stored securely so that they can be restored in case of data loss caused by cyberattacks, hardware failures, or human error. A widely recommended approach is the 3-2-1 backup rule, which involves keeping three copies of data, stored on two different types of media, with one copy kept offsite or in a secure cloud environment. Regularly testing backup systems is essential to ensure that data can be successfully restored when needed. Recovery planning involves having clear procedures in place to quickly restore operations after an incident. Without proper backups, organizations may suffer permanent data loss or be forced to pay ransomware demands. Effective backup and recovery strategies reduce downtime, protect critical information, and support business continuity during unexpected disruptions." },
            { "incident response", "Incident response refers to a structured approach used to detect, manage, and recover from cybersecurity incidents such as data breaches, malware infections, or unauthorized access. A well-developed incident response plan outlines the steps to be taken during an incident, including identification, containment, eradication, recovery, and post-incident analysis. It also defines roles and responsibilities, communication procedures, and methods for preserving evidence. Quick and effective response is essential to minimize damage and prevent the spread of threats within a system. Organizations often establish dedicated incident response teams to handle such situations. After resolving an incident, it is important to conduct a review to identify weaknesses and improve future defenses. Regular training and simulations help ensure readiness. A strong incident response capability enhances an organization’s resilience and ability to recover from cyber threats efficiently." },
            { "privacy settings", "Privacy settings are tools that allow users to control how their personal information is collected, used, and shared by applications, websites, and social media platforms. Properly configuring these settings can significantly reduce the risk of data exposure and identity theft. Users should regularly review permissions granted to apps, such as access to location, contacts, camera, and microphone, and disable any that are not necessary. Many platforms offer options to limit who can see your posts, profile information, and activity. It is also advisable to disable features that track your behavior for advertising purposes. Default settings are often less secure, so adjusting them to more privacy-friendly options is important. Staying informed about privacy policies and updates helps users make better decisions about their data. Managing privacy settings effectively is a simple yet powerful way to enhance personal security online." },
            { "iot security", "Internet of Things (IoT) security focuses on protecting connected devices such as smart home systems, cameras, wearable devices, and appliances from cyber threats. These devices often have limited built-in security, making them attractive targets for attackers. One of the most important steps is to change default usernames and passwords, as many devices come with weak credentials that are widely known. Keeping device firmware updated ensures that security vulnerabilities are patched. It is also recommended to place IoT devices on a separate network or VLAN to prevent attackers from accessing more critical systems if one device is compromised. Disabling unnecessary features and monitoring device activity can help detect unusual behavior. As the number of connected devices continues to grow, ensuring proper IoT security is essential to prevent unauthorized access and maintain overall network safety." },
            { "mobile security", "Mobile security involves protecting smartphones and tablets from threats such as malware, data theft, and unauthorized access. Since mobile devices store a large amount of personal and sensitive information, they are a common target for cybercriminals. Users should always keep their operating system and applications updated to fix security vulnerabilities. Installing apps only from trusted sources such as official app stores reduces the risk of downloading malicious software. Enabling screen locks, biometric authentication, and device encryption adds an extra layer of protection. It is also important to review app permissions and avoid granting unnecessary access to sensitive data. Avoid connecting to unsecured public Wi-Fi networks or use a VPN for protection. For organizations, mobile device management systems can enforce security policies. Practicing good mobile security habits helps protect both personal and professional data from cyber threats." },
            { "secure coding", "Secure coding is the practice of writing software in a way that minimizes vulnerabilities and protects against cyberattacks. Developers must follow best practices such as validating all user input to prevent attacks like SQL injection and cross-site scripting. Using parameterized queries and prepared statements helps ensure that malicious input does not compromise the system. The principle of least privilege should be applied so that users and processes only have access to what they need. Regular code reviews, static analysis, and security testing help identify and fix vulnerabilities early in the development process. Keeping libraries and dependencies updated is also important to avoid known security flaws. Secure coding is not a one-time task but an ongoing process that requires awareness and discipline. By integrating security into every stage of development, organizations can build more reliable and resilient applications that are less susceptible to attacks." },
            { "network segmentation", "Network segmentation is a security strategy that involves dividing a network into smaller, isolated segments to limit the spread of cyber threats. By separating systems based on their function or sensitivity, organizations can control access and reduce the risk of attackers moving laterally across the network. Technologies such as Virtual Local Area Networks (VLANs), firewalls, and access control lists are commonly used to enforce segmentation. For example, sensitive data systems can be isolated from general user networks, and IoT devices can be placed on separate segments. This approach ensures that even if one part of the network is compromised, the rest remains protected. Proper configuration and monitoring are essential to maintain effective segmentation. Network segmentation is a key component of modern cybersecurity frameworks and plays a significant role in enhancing overall network security." },
            { "access control", "Access control is a security mechanism that ensures users and systems can only access resources that they are authorized to use. It is based on the principle of least privilege, which means granting the minimum level of access necessary for a user to perform their tasks. Common models include role-based access control (RBAC) and attribute-based access control (ABAC), which assign permissions based on roles or specific attributes. Strong authentication methods, such as multi-factor authentication, are used to verify user identities. Regular audits and reviews of access rights are important to remove unnecessary permissions and prevent misuse. Access control also involves monitoring and logging user activity to detect suspicious behavior. By properly managing access, organizations can protect sensitive information, reduce the risk of insider threats, and maintain a secure environment." },
            { "security policies", "Security policies are formal documents that define the rules, guidelines, and procedures for maintaining an organization’s security posture. They cover areas such as acceptable use of systems, password requirements, data protection, incident response, and employee responsibilities. Well-defined policies help ensure that all users understand their role in protecting organizational assets. Policies should be clear, enforceable, and aligned with legal and regulatory requirements. Regular reviews and updates are necessary to address emerging threats and changes in technology. Training and awareness programs are essential to ensure compliance. Without proper enforcement, even the best policies are ineffective. Security policies provide a foundation for consistent decision-making and help organizations respond effectively to security incidents while maintaining compliance and reducing risk." }
        };

        string lower = (input ?? string.Empty).ToLower();

        if (lower.Contains("what can i ask") || lower.Contains("what can i ask you") || lower.Contains("what can i ask about") || lower.Contains("what can you answer"))
        {
            return $"{UserStore.CurrentUserName}, You can ask me about: passwords, phishing, malware, social engineering, two-factor authentication (2FA), safe browsing, ransomware, public Wi-Fi safety, identity theft, software updates, encryption, firewalls, backup & recovery, incident response, privacy settings, IoT security, mobile security, secure coding, network segmentation, access control, and security policies.";
        }

        // numeric shortcuts
        if (lower == "1" || lower == "password") return UserStore.CurrentUserName + ", " + topics["password"];
        if (lower == "2" || lower.Contains("phish")) return UserStore.CurrentUserName + ", " + topics["phishing"];
        if (lower == "3" || lower == "malware") return UserStore.CurrentUserName + ", " + topics["malware"];
        if (lower == "4" || lower.Contains("social")) return UserStore.CurrentUserName + ", " + topics["social engineering"];
        if (lower == "5" || lower == "2fa") return UserStore.CurrentUserName + ", " + topics["2fa"];
        if (lower == "6" || lower.Contains("browse")) return UserStore.CurrentUserName + ", " + topics["safe browsing"];
        if (lower == "7" || lower == "ransomware") return UserStore.CurrentUserName + ", " + topics["ransomware"];
        if (lower == "8" || lower.Contains("wifi")) return UserStore.CurrentUserName + ", " + topics["public wifi"];
        if (lower == "9" || lower.Contains("identity")) return UserStore.CurrentUserName + ", " + topics["identity theft"];
        if (lower == "10" || lower.Contains("update") || lower.Contains("patch")) return UserStore.CurrentUserName + ", " + topics["updates"];
        if (lower == "11" || lower.Contains("encrypt")) return UserStore.CurrentUserName + ", " + topics["encryption"];
        if (lower == "12" || lower.Contains("firewall")) return UserStore.CurrentUserName + ", " + topics["firewalls"];
        if (lower == "13" || lower.Contains("backup") || lower.Contains("recovery")) return UserStore.CurrentUserName + ", " + topics["backup & recovery"];
        if (lower == "14" || lower.Contains("incident")) return UserStore.CurrentUserName + ", " + topics["incident response"];
        if (lower == "15" || lower.Contains("privacy")) return UserStore.CurrentUserName + ", " + topics["privacy settings"];
        if (lower == "16" || lower.Contains("iot")) return UserStore.CurrentUserName + ", " + topics["iot security"];
        if (lower == "17" || lower.Contains("mobile")) return UserStore.CurrentUserName + ", " + topics["mobile security"];
        if (lower == "18" || lower.Contains("secure code") || lower.Contains("secure coding")) return UserStore.CurrentUserName + ", " + topics["secure coding"];
        if (lower == "19" || lower.Contains("segment")) return UserStore.CurrentUserName + ", " + topics["network segmentation"];
        if (lower == "20" || lower.Contains("access control") || lower.Contains("access")) return UserStore.CurrentUserName + ", " + topics["access control"];
        if (lower == "21" || lower.Contains("policy")) return UserStore.CurrentUserName + ", " + topics["security policies"];

        // tokenization and fuzzy suggestions
        var tokens = new System.Collections.Generic.List<string>();
        foreach (var t in System.Text.RegularExpressions.Regex.Split(lower, "[^a-z0-9]+")) if (!string.IsNullOrEmpty(t)) tokens.Add(t);

        var matched = new System.Collections.Generic.List<string>();
        foreach (var kv in topics)
        {
            if (lower.Contains(kv.Key)) { matched.Add(kv.Key); continue; }
            var keyTokens = kv.Key.Split(' ');
            foreach (var kt in keyTokens) if (tokens.Contains(kt)) { matched.Add(kv.Key); break; }
        }

        if (matched.Count > 0) return UserStore.CurrentUserName + ", " + string.Join("\n\n", matched.ConvertAll(k => topics[k]));

        var suggestions = new System.Collections.Generic.List<string>();
        foreach (var kv in topics)
        {
            var keyTokens = kv.Key.Split(' ');
            foreach (var kt in keyTokens)
                foreach (var it in tokens)
                {
                    int dist = Chatbot.LevenshteinDistance(kt, it);
                    int threshold = Math.Max(1, kt.Length / 3);
                    if (dist <= threshold && !suggestions.Contains(kv.Key)) suggestions.Add(kv.Key);
                }
        }

        if (suggestions.Count > 0)
        {
            foreach (var s in suggestions)
            {
                Chatbot.TypeText($"I think you meant \"{s}\". Please type 'yes' or 'no':");
                Console.Write("Your choice: ");
                string reply = Console.ReadLine()?.Trim().ToLower();
                if (!string.IsNullOrEmpty(reply) && (reply.StartsWith("y") || reply == "yes")) return UserStore.CurrentUserName + ", " + topics[s];
            }
        }

        return "I didn’t quite understand that. Could you please rephrase your question or choose a number from the menu? You can type topics such as 'phishing', 'malware', or 'password', or simply enter the corresponding number. My goal is to help you stay safe online, so feel free to try again.";
    }
}
