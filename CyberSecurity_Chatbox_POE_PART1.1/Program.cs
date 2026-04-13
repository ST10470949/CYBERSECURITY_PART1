using System;
using System.IO;
using System.Media;
using System.Threading;

// Cyber Security Chatbot
// Author: (student)
// Description: Console-based cybersecurity awareness assistant. Provides a boot
// sequence, optional voice greeting, ASCII logo, personalized greetings, and 
// a menu-driven chatbot with free-form question handling and fuzzy matching for
// misspellings. Persistence used to welcome returning users.

// Main program class. It runs the app and ties helpers together.
class Program
{
    // Store the current user's name globally so nested classes can access it
    static string CurrentUserName = string.Empty;

    // Answer helpers moved to Chatbot.cs

    // Application entry point. Starts boot, plays audio, then runs chat.
    static void Main()
    {
        // Set the console window title
        Console.Title = "Cyber Times With Abo";

        // Run startup visual effects and media
        BootSequence();
        // Play optional voice greeting after the boot animation (before logo)
        AudioPlayer.PlayIfExists("Abos.wav");
        // Display logo
        DisplayLogo();

        // Ask for the user's name and greet them
        string userName = AskUserName();
        // Save globally for personalized replies
        CurrentUserName = userName;
        ShowProfessionalGreeting(userName);

        // Start the main chat loop (delegated to Chatbot helper)
        Chatbot.StartChat(userName);
    }

    // =========================================================
    // 🚀 BOOT SEQUENCE — VISUAL MEDIA EFFECT
    // =========================================================
    // Show boot messages and simple progress animation.
    static void BootSequence()
    {
        // Use green console color for boot messages
        Console.ForegroundColor = ConsoleColor.Green;

        // Animated typing messages to make startup appear dynamic (keep in green)
        TypeTextColor("Initializing Cyber Times Security System...", ConsoleColor.Green);
        TypeTextColor("Loading threat intelligence modules...", ConsoleColor.Green);
        TypeTextColor("Establishing secure environment...", ConsoleColor.Green);

        // Simple progress dots
        Console.Write("\nSystem booting");
        for (int i = 0; i < 5; i++)
        {
            Console.Write(".");
            Thread.Sleep(400); // pause for effect
        }

        Console.WriteLine("\nSystem ready.\n");
        Console.ResetColor();
    }

    // =========================================================
    // External Chatbot Integration (added without changing existing logic)
    // The user's standalone chatbot code was adapted into this helper class.
    // This class is used when the user selects Extended Topics (option 9).
    // =========================================================
    // Helper for the extended external chatbot menu.
    static class ExternalChatbot
    {
        // Run the external chatbot loop (menu + input handling).
        public static void Run()
        {
            // Header for the external chatbot session
            Console.WriteLine("=== Cybersecurity Awareness Chatbot ===");
            Console.WriteLine("Type a topic number or keyword. Type 'exit' to quit.\n");

            while (true)
            {
                ShowMenuForExternal();

                Console.Write("\nYour choice: ");
                // Read raw input and normalize (trim + lowercase)
                string input = Console.ReadLine()?.Trim().ToLower();

                // ✅ INPUT VALIDATION — handle empty input
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("⚠️ You entered nothing. Please type a number or topic.");
                    continue; // back to menu
                }

                // ✅ Exit option — explicit end of external chatbot
                if (input == "exit")
                {
                    Console.WriteLine("Stay safe online. Returning to main menu...");
                    break;
                }

                // explicit menu choice to return to main menu
                if (input == "0" || input == "back")
                {
                    Console.WriteLine("Returning to main menu...");
                    break;
                }

                // ✅ PROCESS INPUT — get an answer and display it
                string response = GetResponseForExternal(input);

                // Print responses using the same typing animation as the main program
                TypeText(response);
                Console.WriteLine("\n----------------------------------------\n");
            }
        }

        // Show the menu inside the external chatbot
        // Show the extended topics menu.
        static void ShowMenuForExternal()
        {
            // display the same boxed menu layout as the main DisplayMenu (with same color)
            Console.ForegroundColor = ConsoleColor.Yellow;
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

        // This method maps user input (numbers, keywords, or misspellings)
        // to detailed topic responses. It supports fuzzy matching and
        // will prompt the user to confirm suggested corrections.
        // Map a user input to a detailed response. Supports fuzzy matching.
        static string GetResponseForExternal(string input)
        {
            // Build the same topic map used elsewhere so free-form queries and misspellings
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

                { "secure coding", "Secure coding is the practice of writing software in a way that minimizes vulnerabilities and protects against cyberattacks. Developers must follow best practices such as validating all user input to prevent attacks like SQL injection and cross-site scripting. Using parameterized queries and prepared statements helps ensure that malicious input does not compromise the system. The principle of least privilege should be applied so that users and processes only have access to what they need. Regular code reviews, static analysis, and security testing help identify and fix vulnerabilities early in the development process. Keeping libraries and dependencies updated is also important to avoid known security flaws. Secure coding is not a one-time task but an ongoing process that requires awareness and discipline. By integrating security into every stage of development, organizations can build more reliable and resilient applications." },

                { "network segmentation", "Network segmentation is a security strategy that involves dividing a network into smaller, isolated segments to limit the spread of cyber threats. By separating systems based on their function or sensitivity, organizations can control access and reduce the risk of attackers moving laterally across the network. Technologies such as Virtual Local Area Networks (VLANs), firewalls, and access control lists are commonly used to enforce segmentation. For example, sensitive data systems can be isolated from general user networks, and IoT devices can be placed on separate segments. This approach ensures that even if one part of the network is compromised, the rest remains protected. Proper configuration and monitoring are essential to maintain effective segmentation. Network segmentation is a key component of modern cybersecurity frameworks and plays a significant role in enhancing overall network security." },

                { "access control", "Access control is a security mechanism that ensures users and systems can only access resources that they are authorized to use. It is based on the principle of least privilege, which means granting the minimum level of access necessary for a user to perform their tasks. Common models include role-based access control (RBAC) and attribute-based access control (ABAC), which assign permissions based on roles or specific attributes. Strong authentication methods, such as multi-factor authentication, are used to verify user identities. Regular audits and reviews of access rights are important to remove unnecessary permissions and prevent misuse. Access control also involves monitoring and logging user activity to detect suspicious behavior. By properly managing access, organizations can protect sensitive information, reduce the risk of insider threats, and maintain a secure environment." },

                { "security policies", "Security policies are formal documents that define the rules, guidelines, and procedures for maintaining an organization’s security posture. They cover areas such as acceptable use of systems, password requirements, data protection, incident response, and employee responsibilities. Well-defined policies help ensure that all users understand their role in protecting organizational assets. Policies should be clear, enforceable, and aligned with legal and regulatory requirements. Regular reviews and updates are necessary to address emerging threats and changes in technology. Training and awareness programs are essential to ensure that employees follow these policies correctly. Without proper enforcement, even the best policies are ineffective. Security policies provide a foundation for consistent decision-making and help organizations respond effectively to security incidents while maintaining compliance and reducing risk." }
            };



            string lower = (input ?? string.Empty).ToLower();

            // Quick help: if the user asks what they can ask, return a short topic list
            if (lower.Contains("what can i ask") || lower.Contains("what can i ask you") || lower.Contains("what can i ask about") || lower.Contains("what can you answer"))
            {
                return $"{Program.CurrentUserName}, You can ask me about: passwords, phishing, malware, social engineering, two-factor authentication (2FA), safe browsing, ransomware, public Wi-Fi safety, identity theft, software updates, encryption, firewalls, backup & recovery, incident response, privacy settings, IoT security, mobile security, secure coding, network segmentation, access control, and security policies.";
            }

            // Handle questions like "what can you advise me about X" or multi-topic questions
            var adviseMatch = System.Text.RegularExpressions.Regex.Match(lower, "what can (?:you|u) (?:advice|advise) me about(?: the)?\\s+(.+)");
            if (adviseMatch.Success)
            {
                var requested = adviseMatch.Groups[1].Value.Trim();
                // Split requested into possible multiple topics by commas, 'and', or 'then'
                var parts = System.Text.RegularExpressions.Regex.Split(requested, "\\s*(?:,|\\band\\b|\\bthen\\b)\\s*");
                var answers = new System.Collections.Generic.List<string>();
                    foreach (var p in parts)
                {
                    var token = System.Text.RegularExpressions.Regex.Replace(p.Trim(), "[^a-z0-9 ]+", "");
                    if (string.IsNullOrEmpty(token)) continue;
                        if (topics.ContainsKey(token)) { answers.Add(topics[token]); continue; }
                    // try token match within topic keys (token may be a single word)
                    bool found = false;
                            foreach (var kv in topics)
                    {
                                if (kv.Key.Equals(token, System.StringComparison.OrdinalIgnoreCase) || kv.Key.Contains(token)) { answers.Add(kv.Value); found = true; break; }
                        var keyTokens = kv.Key.Split(' ');
                        foreach (var kt in keyTokens)
                        {
                            if (kt.Equals(token, System.StringComparison.OrdinalIgnoreCase)) { answers.Add(kv.Value); found = true; break; }
                        }
                        if (found) break;
                    }
                }
                if (answers.Count > 0) return Program.CurrentUserName + ", " + string.Join("\n\n", answers);
                // fallback: list available topics
                return Program.CurrentUserName + ", I can advise about: " + string.Join(", ", topics.Keys);
            }

            // handle numeric menu choices quickly (keeps existing number shortcuts)
                    if (lower == "1" || lower == "password") return Program.CurrentUserName + ", " + topics["password"];
            if (lower == "2" || lower.Contains("phish")) return Program.CurrentUserName + ", " + topics["phishing"];
            if (lower == "3" || lower == "malware") return Program.CurrentUserName + ", " + topics["malware"];
            if (lower == "4" || lower.Contains("social")) return Program.CurrentUserName + ", " + topics["social engineering"];
            if (lower == "5" || lower == "2fa") return Program.CurrentUserName + ", " + topics["2fa"];
            if (lower == "6" || lower.Contains("browse")) return Program.CurrentUserName + ", " + topics["safe browsing"];
            if (lower == "7" || lower == "ransomware") return Program.CurrentUserName + ", " + topics["ransomware"];
            if (lower == "8" || lower.Contains("wifi")) return Program.CurrentUserName + ", " + topics["public wifi"];
            if (lower == "9" || lower.Contains("identity")) return Program.CurrentUserName + ", " + topics["identity theft"];
            if (lower == "10" || lower.Contains("update") || lower.Contains("patch")) return Program.CurrentUserName + ", " + topics["updates"];
            if (lower == "11" || lower.Contains("encrypt")) return Program.CurrentUserName + ", " + topics["encryption"];
            if (lower == "12" || lower.Contains("firewall")) return Program.CurrentUserName + ", " + topics["firewalls"];
            if (lower == "13" || lower.Contains("backup") || lower.Contains("recovery")) return Program.CurrentUserName + ", " + topics["backup & recovery"];
            if (lower == "14" || lower.Contains("incident")) return Program.CurrentUserName + ", " + topics["incident response"];
            if (lower == "15" || lower.Contains("privacy")) return Program.CurrentUserName + ", " + topics["privacy settings"];
            if (lower == "16" || lower.Contains("iot")) return Program.CurrentUserName + ", " + topics["iot security"];
            if (lower == "17" || lower.Contains("mobile")) return Program.CurrentUserName + ", " + topics["mobile security"];
            if (lower == "18" || lower.Contains("secure code") || lower.Contains("secure coding")) return Program.CurrentUserName + ", " + topics["secure coding"];
            if (lower == "19" || lower.Contains("segment")) return Program.CurrentUserName + ", " + topics["network segmentation"];
            if (lower == "20" || lower.Contains("access control") || lower.Contains("access")) return Program.CurrentUserName + ", " + topics["access control"];
            if (lower == "21" || lower.Contains("policy")) return Program.CurrentUserName + ", " + topics["security policies"];

            // Tokenize input into lowercase words/numbers for matching
            var tokens = new System.Collections.Generic.List<string>();
            foreach (var t in System.Text.RegularExpressions.Regex.Split(lower, "[^a-z0-9]+"))
            {
                if (!string.IsNullOrEmpty(t)) tokens.Add(t);
            }

            // Attempt exact or token-based matches first (handles multi-topic queries)
            var matched = new System.Collections.Generic.List<string>();
            foreach (var kv in topics)
            {
                if (lower.Contains(kv.Key)) { matched.Add(kv.Key); continue; }
                var keyTokens = kv.Key.Split(' ');
                foreach (var kt in keyTokens)
                {
                    if (tokens.Contains(kt)) { matched.Add(kv.Key); break; }
                }
            }

                if (matched.Count > 0)
                {
                    // Combine multiple answers for multi-topic queries and personalize
                    return Program.CurrentUserName + ", " + string.Join("\n\n", matched.ConvertAll(k => topics[k]));
                }

            // Fuzzy suggestions using Levenshtein to detect simple misspellings
            var suggestions = new System.Collections.Generic.List<string>();
            foreach (var kv in topics)
            {
                var keyTokens = kv.Key.Split(' ');
                foreach (var kt in keyTokens)
                {
                    foreach (var it in tokens)
                    {
                        int dist = LevenshteinDistance(kt, it);
                        int threshold = Math.Max(1, kt.Length / 3); // tolerance based on token length
                        if (dist <= threshold)
                        {
                            if (!suggestions.Contains(kv.Key)) suggestions.Add(kv.Key);
                        }
                    }
                }
            }
                        if (suggestions.Count > 0)
                        {
                            // Ask user to confirm possible corrections (yes/no)
                            foreach (var s in suggestions)
                            {
                                TypeText($"I think you meant \"{s}\". Please type 'yes' or 'no':");
                                Console.Write("Your choice: ");
                                string reply = Console.ReadLine()?.Trim().ToLower();
                                if (!string.IsNullOrEmpty(reply) && (reply.StartsWith("y") || reply == "yes"))
                                {
                                    return Program.CurrentUserName + ", " + topics[s];
                                }
                            }
                        }

            if (suggestions.Count > 0)
            {
                // Ask user to confirm possible corrections (yes/no)
                foreach (var s in suggestions)
                {
                    TypeText($"I think you meant \"{s}\". Please type 'yes' or 'no':");
                    Console.Write("Your choice: ");
                    string reply = Console.ReadLine()?.Trim().ToLower();
                    if (!string.IsNullOrEmpty(reply) && (reply.StartsWith("y") || reply == "yes"))
                    {
                        return Program.CurrentUserName + ", " + topics[s];
                    }
                }
            }

            // Default fallback when nothing matches or user declines suggestions
            return "I didn’t quite understand that. Could you please rephrase your question or choose a number from the menu? You can type topics such as 'phishing', 'malware', or 'password', or simply enter the corresponding number. My goal is to help you stay safe online, so feel free to try again.";
        }
    }

    
    // =========================================================
    // 🌐 CREATIVE ASCII LOGO
    // =========================================================
    // Print the ASCII logo framed box.
    static void DisplayLogo()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;

        // Large ASCII art block — displayed at startup
        Console.WriteLine(@"
╔════════════════════════════════════════════════════════════════╗
║   ██████╗██╗   ██╗██████╗ ███████╗██████╗                      ║
║  ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗                     ║
║  ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝                     ║
║  ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗                     ║
║  ╚██████╗   ██║   ██████╔╝███████╗██║  ██║                     ║
║   ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝                     ║
║                                                                ║
║   ████████╗██╗███╗   ███╗███████╗███████╗                      ║
║   ╚══██╔══╝██║████╗ ████║██╔════╝██╔════╝                      ║
║      ██║   ██║██╔████╔██║█████╗  ███████╗                      ║
║      ██║   ██║██║╚██╔╝██║██╔══╝  ╚════██║                      ║
║      ██║   ██║██║ ╚═╝ ██║███████╗███████║                      ║
║      ╚═╝   ╚═╝╚═╝     ╚═╝╚══════╝╚══════╝                      ║
║                                                                ║
║                 CYBER - TIMES WITH ABO                         ║
║          Cybersecurity Awareness Assistant                     ║
║                                                                ║
║         Stay Alert. Stay Secure. Stay Ahead.                   ║
╚════════════════════════════════════════════════════════════════╝
");

        Console.ResetColor();
    }

    // =========================================================
    // 👤 ASK USER NAME WITH VALIDATION
    // =========================================================
    // Prompt user for name, validate, and update usage counts.
    static string AskUserName()
    {
        string name;

        // persistent usage counts stored in a simple text file in app directory
        //it is set to save the credentials of the user and the number of times they have used the system, this is done to make the system more personalized and to give a friendly greeting to returning users. The file is named "user_counts.txt" and is located in the same directory as the application.
        string countsPath = Path.Combine(AppContext.BaseDirectory, "user_counts.txt");

        while (true)
        {
            Console.Write("\nPlease enter your name: ");
            name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
                break; // valid input

            // Friendly error message for empty input
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Name cannot be empty. Please try again.");
            Console.ResetColor();
        }

        name = name.Trim();

        try
        {
            // Load counts, increment for this user, and save back to file
            var counts = LoadUserCounts(countsPath);
            counts.TryGetValue(name, out int prev);
            int updated = prev + 1;
            counts[name] = updated;
            SaveUserCounts(countsPath, counts);

            // if user has used the system at least once before (this is now 2nd or more visit)
            if (prev >= 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                TypeText($"Hey, welcome back {name}.");
                Console.ResetColor();
            }
        }
        catch
        {
            // non-fatal: ignore persistence errors and continue
        }

        return name;
    }

    // Simple persistent storage helpers (format: name:count per line)
    // Load name:count pairs from a file into a dictionary.
    static System.Collections.Generic.Dictionary<string, int> LoadUserCounts(string path)
    {
        var dict = new System.Collections.Generic.Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(path)) return dict;

        // Each line expected as "name:count"
        foreach (var line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var idx = line.LastIndexOf(':');
            if (idx <= 0) continue;
            var n = line.Substring(0, idx).Trim();
            var s = line.Substring(idx + 1).Trim();
            if (int.TryParse(s, out int v)) dict[n] = v;
        }

        return dict;
    }

    // Save the name:count dictionary back to disk.
    static void SaveUserCounts(string path, System.Collections.Generic.Dictionary<string, int> dict)
    {
        try
        {
            var lines = new System.Collections.Generic.List<string>();
            foreach (var kv in dict)
            {
                lines.Add($"{kv.Key}:{kv.Value}");
            }
            File.WriteAllLines(path, lines);
        }
        catch
        {
            // ignore write failures to avoid crashing the app
        }
    }

    // =========================================================
    // 👋 PROFESSIONAL GREETING
    // =========================================================
    // Show a short professional greeting to the named user.
    static void ShowProfessionalGreeting(string name)
    {
        Console.ForegroundColor = ConsoleColor.Green;

        // Several TypeText calls to make greeting feel friendly and paced
        TypeText($"\nWelcome, {name}.");
        TypeText("It is a pleasure to have you here.");
        TypeText("I am your Cyber Times assistant, designed to help you understand cybersecurity risks and protect yourself online.");
        TypeText("You may ask about passwords, scams, malware, privacy, or safe browsing.");

        Console.ResetColor();
    }

    // =========================================================
    // 💬 CHAT SYSTEM — DEPTH & VARIETY
    // =========================================================
    // Older StartChat method (kept for compatibility). Shows menu and handles input.
    static void StartChat(string name)
    {
        while (true)
        {
            DisplayMenu();

            Console.Write("\nYour choice: ");
            string input = Console.ReadLine()?.Trim();

            // Shared topic map used by numeric menu choices and free-form queries
            var menuTopics = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
            {
                { "password", "Strong password security is one of the most critical aspects of protecting your digital identity in today’s interconnected world. A secure password should be long, ideally between 12 and 16 characters or more, and include a combination of uppercase letters, lowercase letters, numbers, and special symbols. This complexity makes it significantly harder for attackers to crack passwords using brute-force or dictionary attacks. Avoid using personal information such as your name, surname, birthdate, or common words, as these are easy for attackers to guess using social engineering or automated tools. Each account should have a unique password, because reusing passwords across multiple platforms increases the risk of widespread compromise if one account is breached. Password managers are highly recommended because they can generate strong passwords and store them securely. Additionally, regularly updating passwords and monitoring account activity helps detect unauthorized access early. Combining strong passwords with two-factor authentication adds an extra layer of security, making it far more difficult for attackers to gain access." },

                { "phishing", "Phishing is a deceptive cyberattack method used by criminals to trick individuals into revealing sensitive information such as usernames, passwords, banking details, or personal identification data. These attacks often appear as legitimate communications from trusted organizations like banks, government agencies, or popular online platforms. Phishing messages are typically delivered via email, SMS, or fake websites that closely resemble real ones. Attackers frequently create a sense of urgency, such as warning about account suspension or suspicious activity, to pressure victims into acting quickly without verifying the authenticity of the request. Common warning signs include poor grammar, unfamiliar sender addresses, suspicious links, and unexpected attachments. To protect yourself, always verify the sender’s identity and avoid clicking on links from unknown or untrusted sources. Instead, manually type the official website address into your browser. Using spam filters and security tools can help detect phishing attempts. Ultimately, awareness and cautious behavior are the most effective defenses against phishing attacks." },

                { "malware", "Malware refers to malicious software designed to harm, exploit, or gain unauthorized access to computer systems and networks. Common types include viruses, worms, trojans, spyware, ransomware, and adware. Malware can infect devices through various methods, such as downloading files from untrusted sources, opening infected email attachments, clicking malicious links, or using compromised removable media. Once installed, malware can perform harmful activities like stealing sensitive information, monitoring user behavior, corrupting files, slowing system performance, or granting attackers remote control of the device. Some malware operates silently, making it difficult to detect without specialized tools. To defend against malware, users should install reputable antivirus and anti-malware software and keep it updated regularly. Avoid downloading software from unknown websites and always scan files before opening them. Keeping your operating system and applications updated is essential because updates patch vulnerabilities that malware often exploits. Practicing safe browsing habits and being cautious with email attachments significantly reduces the risk of infection." },

                { "social engineering", "Social engineering is a cyberattack technique that manipulates individuals into revealing confidential information or performing actions that compromise security. Instead of targeting technical vulnerabilities, attackers exploit human psychology, using tactics such as fear, urgency, curiosity, or authority to influence behavior. For example, an attacker may impersonate a bank official, IT technician, or colleague to gain trust and request sensitive information like passwords or financial details. These attacks can occur through emails, phone calls, text messages, or even in person. Social engineering is particularly dangerous because it relies on human error rather than system weaknesses. To protect yourself, always verify the identity of anyone requesting sensitive information, especially if the request seems urgent or unusual. Never share passwords or confidential data over unsecured channels. Organizations typically have policies that prevent employees from asking for such information directly. Awareness, skepticism, and proper training are essential in preventing social engineering attacks and protecting sensitive information." },

                { "2fa", "Two-Factor Authentication (2FA) is a security measure that adds an additional layer of protection to online accounts by requiring two forms of verification. Instead of relying solely on a password, users must provide a second factor, such as a one-time code sent to their phone, a code generated by an authentication app, or biometric verification like a fingerprint or facial recognition. This ensures that even if a password is compromised, unauthorized users cannot access the account without the second factor. 2FA is especially important for sensitive accounts such as email, banking, and social media platforms. Authentication apps are generally more secure than SMS-based codes, as they are less vulnerable to interception or SIM-swapping attacks. Enabling 2FA significantly reduces the risk of unauthorized access and is considered a best practice in cybersecurity. Many online services now offer 2FA, and users are strongly encouraged to enable it wherever possible for enhanced protection." },

                { "safe browsing", "Safe browsing involves adopting practices that protect users from online threats while navigating the internet. One key aspect is ensuring that websites use HTTPS, which indicates a secure, encrypted connection between the user and the website. Users should avoid downloading files or software from unknown or untrusted sources, as these may contain malware. Pop-ups that claim your device is infected or urge immediate action are often scams and should be ignored. Modern web browsers include built-in security features that help detect and block malicious websites, making it important to keep browsers updated. Additionally, browser extensions that block ads, trackers, and harmful scripts can enhance security. Users should also be cautious when entering personal information online and ensure they are on legitimate websites. Practicing safe browsing habits reduces the risk of cyberattacks, protects personal data, and ensures a safer online experience overall." },

                { "ransomware", "Ransomware is a type of malicious software that encrypts a victim’s files and demands payment, typically in cryptocurrency, to restore access. Once infected, users are often locked out of their systems and presented with a ransom note containing payment instructions. However, paying the ransom does not guarantee that access will be restored and may encourage further criminal activity. Ransomware commonly spreads through phishing emails, malicious attachments, or compromised websites. The best defense against ransomware is prevention, including regularly backing up important data using the 3-2-1 rule and storing backups securely offline or in the cloud. Users should avoid opening suspicious emails or attachments and ensure their security software and operating systems are up to date. If an infection occurs, the affected device should be disconnected from the network immediately to prevent the spread. Strong cybersecurity practices significantly reduce the risk of ransomware attacks." },

                { "public wifi", "Public Wi-Fi networks, such as those in cafes, airports, and hotels, provide convenience but often lack proper security measures. This makes them vulnerable to cyberattacks, where attackers can intercept data transmitted over the network. Techniques such as man-in-the-middle attacks allow cybercriminals to eavesdrop on user activity and capture sensitive information like passwords and financial details. To stay safe, users should avoid accessing sensitive accounts while connected to public Wi-Fi. If necessary, a Virtual Private Network (VPN) should be used to encrypt internet traffic and protect data from interception. Users should also disable automatic connections to open networks and turn off file sharing features. Verifying the legitimacy of the network before connecting is important, as attackers may create fake hotspots. Practicing caution when using public Wi-Fi helps protect personal information and reduces the risk of cyber threats." },

                { "identity theft", "Identity theft occurs when criminals steal personal information and use it to commit fraud or other illegal activities. This may include opening bank accounts, applying for loans, or making purchases in the victim’s name. Personal information can be obtained through phishing attacks, data breaches, social media oversharing, or physical theft. The consequences of identity theft can be severe, including financial loss and damage to credit records. To protect against identity theft, individuals should limit the amount of personal information shared online and regularly monitor financial statements for suspicious activity. Strong passwords and two-factor authentication should be used to secure accounts. If identity theft is suspected, it should be reported immediately to banks and relevant authorities to minimize damage. Awareness and proactive monitoring are essential in preventing and managing identity theft risks." },

                { "updates", "Software updates are essential for maintaining the security and functionality of devices and applications. Developers regularly release updates to fix bugs, improve performance, and patch security vulnerabilities that attackers can exploit. Using outdated software increases the risk of cyberattacks, as known vulnerabilities are often targeted by hackers. Enabling automatic updates ensures that systems remain protected without requiring manual intervention. Updates may also introduce new features and improve overall system stability. Users should not ignore update notifications, especially for critical software such as operating systems and antivirus programs. Regularly checking for and installing updates is one of the simplest yet most effective ways to enhance cybersecurity. Keeping systems up to date reduces exposure to threats and ensures a safer computing environment." },

                { "encryption", "Encryption is a critical cybersecurity technique used to protect sensitive data by converting it into an unreadable format that can only be accessed with the correct decryption key. This ensures that even if data is intercepted during transmission or accessed without authorization, it cannot be understood or misused. Encryption is widely used in securing online communications, financial transactions, emails, and stored data. There are two main types of encryption: symmetric encryption, which uses a single shared key, and asymmetric encryption, which uses a pair of keys known as the public and private keys. Common encryption standards include AES for securing stored data and TLS for protecting data transmitted over the internet. However, encryption is only effective when combined with proper key management, as exposed or weak keys can compromise the entire system. Organizations must also ensure that encryption is applied consistently across all systems handling sensitive information. When integrated with access controls and authentication mechanisms, encryption provides a strong layer of defense against data breaches and unauthorized access." },

                { "firewalls", "Firewalls are essential security tools that monitor and control incoming and outgoing network traffic based on predefined security rules. They act as a protective barrier between a trusted internal network and untrusted external networks, such as the internet. Firewalls can be hardware-based, software-based, or a combination of both, and they are used to block unauthorized access while allowing legitimate communication. They analyze data packets and determine whether to allow or block them based on criteria such as IP addresses, ports, and protocols. Advanced firewalls may also include intrusion detection and prevention capabilities, which help identify and stop potential attacks in real time. Proper configuration is crucial, as poorly configured firewalls can leave systems vulnerable. Regularly reviewing and updating firewall rules ensures they remain effective against evolving threats. Firewalls are a fundamental component of network security and are often used alongside other security measures such as antivirus software and encryption to create a comprehensive defense strategy." },

                { "backup & recovery", "Backup and recovery strategies are essential for protecting data against loss caused by cyberattacks, hardware failures, or human error. Backups involve creating copies of important data and storing them in secure locations so they can be restored if needed. A widely recommended approach is the 3-2-1 rule, which means keeping three copies of data, stored on two different types of media, with one copy stored offsite or in the cloud. Regular backups ensure that even in the event of ransomware attacks or system failures, data can be recovered without significant disruption. Recovery planning involves having clear procedures in place to restore systems and data quickly after an incident. It is important to test backup and recovery processes regularly to ensure they work as expected. Without proper backups, organizations risk permanent data loss and operational downtime. Effective backup and recovery strategies are critical for maintaining business continuity and minimizing the impact of unexpected events." },

                { "incident response", "Incident response is a structured approach used to manage and respond to cybersecurity incidents such as data breaches, malware infections, or unauthorized access. A well-defined incident response plan outlines the steps to identify, contain, eliminate, and recover from security incidents. It also defines roles and responsibilities, communication procedures, and methods for preserving evidence for investigation. Quick and effective response is essential to minimize damage and prevent the spread of threats. Incident response typically involves several phases, including preparation, detection, containment, eradication, recovery, and post-incident analysis. After resolving an incident, organizations should review what happened and implement improvements to prevent similar incidents in the future. Regular training and simulation exercises help ensure that teams are prepared to respond effectively. A strong incident response capability enhances an organization’s resilience and reduces the overall impact of cyber threats." },

                { "privacy settings", "Privacy settings are tools that allow users to control how their personal information is collected, used, and shared by online platforms and applications. Properly configuring these settings is essential for protecting personal data and reducing the risk of identity theft or unauthorized access. Users should regularly review app permissions and disable access to features such as location, camera, or contacts unless necessary. Social media platforms often provide options to control who can view posts, send messages, or access profile information. Default settings are often less secure, so adjusting them to more restrictive options is recommended. Users should also disable unnecessary tracking features used for advertising purposes. Staying informed about privacy policies and updates helps users make better decisions about their data. Managing privacy settings effectively is a simple but powerful way to enhance personal security and maintain control over personal information in the digital environment." },

                { "iot security", "Internet of Things (IoT) security focuses on protecting connected devices such as smart home systems, security cameras, and wearable devices from cyber threats. These devices often have limited security features, making them attractive targets for attackers. One of the most important steps in securing IoT devices is changing default usernames and passwords, as these are commonly known and easily exploited. Keeping device firmware updated ensures that known vulnerabilities are patched. It is also recommended to place IoT devices on a separate network to prevent attackers from accessing critical systems if one device is compromised. Disabling unnecessary features and monitoring device activity can help detect suspicious behavior. As the number of IoT devices continues to grow, securing them becomes increasingly important. Proper IoT security practices help prevent unauthorized access and protect both personal data and network integrity." },

                { "mobile security", "Mobile security involves protecting smartphones and tablets from threats such as malware, data theft, and unauthorized access. Mobile devices store a large amount of personal and sensitive information, making them attractive targets for cybercriminals. Users should keep their operating systems and applications updated to fix vulnerabilities. Installing apps only from trusted sources, such as official app stores, reduces the risk of downloading malicious software. Enabling screen locks, biometric authentication, and device encryption adds an extra layer of protection. It is also important to review app permissions and avoid granting unnecessary access to sensitive data. Users should avoid connecting to unsecured public Wi-Fi networks or use a VPN when necessary. Organizations may implement mobile device management systems to enforce security policies. Practicing good mobile security habits helps protect personal and organizational data from cyber threats." },

                { "secure coding", "Secure coding is the practice of developing software in a way that minimizes vulnerabilities and protects against cyberattacks. Developers must validate all user inputs to prevent attacks such as SQL injection and cross-site scripting. Using parameterized queries and secure coding frameworks helps ensure that applications handle data safely. The principle of least privilege should be applied to restrict access to only what is necessary. Regular code reviews, static analysis, and security testing help identify vulnerabilities early in the development process. Keeping libraries and dependencies updated is also important to avoid known security flaws. Secure coding requires continuous awareness and adherence to best practices throughout the software development lifecycle. By integrating security into development processes, organizations can build more reliable and resilient applications that are less susceptible to attacks." },

                { "network segmentation", "Network segmentation is a cybersecurity strategy that divides a network into smaller, isolated segments to improve security and control. By separating systems based on function or sensitivity, organizations can limit the spread of cyber threats and reduce the risk of attackers moving laterally across the network. Technologies such as VLANs, firewalls, and access control lists are used to enforce segmentation. For example, sensitive systems can be isolated from general user networks, and IoT devices can be placed on separate segments. This ensures that even if one segment is compromised, the rest of the network remains protected. Proper configuration and monitoring are essential to maintain effective segmentation. Network segmentation is a key component of modern cybersecurity frameworks and enhances overall network resilience." },

                { "access control", "Access control ensures that users and systems can only access resources they are authorized to use. It is based on the principle of least privilege, which means granting only the minimum level of access required to perform tasks. Common models include role-based access control and attribute-based access control. Strong authentication methods, such as multi-factor authentication, help verify user identities. Regular audits and reviews are necessary to remove unnecessary permissions and prevent misuse. Monitoring user activity helps detect suspicious behavior. Effective access control protects sensitive information, reduces insider threats, and ensures a secure environment." },

                { "security policies", "Security policies are formal guidelines that define how an organization protects its information systems and data. They cover areas such as acceptable use, password management, data protection, and incident response. Clear and enforceable policies ensure that all users understand their responsibilities in maintaining security. Policies should align with legal and regulatory requirements and be reviewed regularly to address new threats. Training and awareness programs are essential to ensure compliance. Without enforcement, policies are ineffective. Security policies provide a structured approach to managing risk and maintaining a strong security posture,data protection, incident response, and employee responsibilities. Well-defined policies help ensure that all users understand their role in protecting organizational assets. Policies should be clear, enforceable, and aligned with legal and regulatory requirements. Regular reviews and updates are necessary to address emerging threats and changes in technology. Training and awareness programs are essential to ensure that employees follow these policies correctly. Without proper enforcement, even the best policies are ineffective. Security policies provide a foundation for consistent decision-making and help organizations respond effectively to security incidents while maintaining compliance and reducing risk." },
            };

            if (string.IsNullOrWhiteSpace(input))
            {
                // Prompt user for a valid entry
                ShowError("Please enter a valid option.");
                continue;
            }

            // Handle the user's top-level menu choice.
            // Accepts numeric selections (1-21), '9' to enter extended topics,
            // '8' to exit, or free-form questions processed by fuzzy matching.
            switch (input)
            {
                case "1":
                    // How are you? — show personalized response directly (no BuildAnswer)
                    TypeText($"{Program.CurrentUserName}, I am operating optimally and ready to assist you. I can provide detailed guidance on cybersecurity topics, explain risks, and suggest practical steps to protect your online accounts and devices. Ask me about passwords, phishing, malware, privacy practices, safe browsing, or how to respond to suspicious activity, and I will give clear, actionable advice.");
                    break;

                case "2":
                    // Purpose — show personalized response directly (no BuildAnswer)
                    TypeText($"{Program.CurrentUserName}, My purpose is to educate users about digital threats and promote safe online behaviour. I provide explanations of common attack types, best practices to reduce risk, and step-by-step recommendations you can apply immediately. Whether you need help selecting a password manager, recognizing scam emails, or hardening your personal devices, I aim to make cybersecurity understandable and practical.");
                    break;

                case "3":
                    // Direct response from menuTopics (user can edit text here)
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["password"]);
                    break;

                case "4":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["phishing"]);
                    break;

                case "5":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["safe browsing"]);
                    break;

                case "6":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["malware"]);
                    break;

                case "7":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["public wifi"]);
                    break;

                case "10":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["updates"]);
                    break;

                case "11":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["encryption"]);
                    break;

                case "12":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["firewalls"]);
                    break;

                case "13":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["backup & recovery"]);
                    break;

                case "14":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["incident response"]);
                    break;

                case "15":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["privacy settings"]);
                    break;

                case "16":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["iot security"]);
                    break;

                case "17":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["mobile security"]);
                    break;

                case "18":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["secure coding"]);
                    break;

                case "19":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["network segmentation"]);
                    break;

                case "20":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["access control"]);
                    break;

                case "21":
                    TypeText($"{Program.CurrentUserName}, " + menuTopics["security policies"]);
                    break;

                case "9":
                    // Launch external chatbot integration (preserves existing behavior)
                    // Ensure UserStore is updated so external helper recognizes the current user
                    UserStore.CurrentUserName = CurrentUserName;
                    ExternalChatbot.Run();
                    break;

                case "8":
                    // Exit the program politely
                    ExitProgram(name);
                    return;

                default:
                    // Free-form question handling with fuzzy matching for misspellings.
                    string lower = input.ToLower();

                    // Quick help: respond to "what can I ask you about?" in main chat
                    if (lower.Contains("what can i ask") || lower.Contains("what can i ask you") || lower.Contains("what can i ask about") || lower.Contains("what can you answer"))
                    {
                        TypeText($"{Program.CurrentUserName}, You can ask me about: passwords, phishing, malware, social engineering, two-factor authentication (2FA), safe browsing, ransomware, public Wi-Fi safety, identity theft, software updates, encryption, firewalls, backup & recovery, incident response, privacy settings, IoT security, mobile security, secure coding, network segmentation, access control, and security policies.");
                        break;
                    }

                    // Map canonical topic keys to their full answers
                    var topicsCore = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
                    {
                        { "password", "Strong password security is one of the most important ways to protect your online accounts. A secure password should be long, complex, and unique for each account. Avoid using personal information such as your name or birthdate. Use a mix of uppercase and lowercase letters, numbers, and symbols, or let a password manager generate and store strong random passwords for you. Enable multi-factor authentication where available and change passwords immediately if you suspect a breach. Regularly review account activity and use unique passwords per site to prevent credential reuse attacks." },
                        { "phishing", "Phishing is a deceptive attack where criminals impersonate trusted organizations to steal credentials or personal data. These messages often urge immediate action, contain unexpected attachments, or include links to fake websites. Inspect the sender's address, hover over links to preview URLs, and never enter credentials from an email link—navigate directly to the official site. Be cautious with unsolicited requests for sensitive data and verify by contacting the organization through official channels. Report suspicious messages to your provider or IT team." },
                        { "malware", "Malware is software designed to harm or compromise systems, including viruses, trojans, spyware, and ransomware. It spreads via malicious downloads, infected attachments, or compromised websites. Use updated antivirus software, avoid running unknown executables, and keep your operating system and applications patched. Regularly back up important data to an offline or immutable location. If you suspect infection, disconnect from networks, run scans with trusted tools, and seek professional support to contain and remediate the issue." },
                        { "ransomware", "Ransomware is a type of malware that encrypts your files and demands payment to restore access. Victims are often instructed to pay in cryptocurrency, making it difficult to trace the attackers. Paying the ransom does not guarantee file recovery and encourages further criminal activity. The best defense is prevention: regularly back up important data, avoid suspicious attachments, and keep security software updated. If infected, disconnect from the network immediately to prevent the spread to other devices." },
                        { "public wifi", "Public Wi-Fi networks, such as those in cafes or airports, are convenient but often insecure. Attackers can intercept unencrypted traffic and capture credentials or personal information. Avoid accessing sensitive accounts while connected to public Wi-Fi unless you use a Virtual Private Network (VPN), which encrypts your internet traffic. Also, disable automatic connection to open networks and ensure file sharing is turned off when using public connections." },
                        { "safe browsing", "Safe browsing protects your data while using the web. Always confirm a site uses HTTPS before entering sensitive information, and check for the correct domain to avoid impostor sites. Avoid downloading files or plugins from untrusted sources, be skeptical of pop-ups claiming your device is infected, and keep your browser updated so built-in protections can block malicious pages. Consider browser extensions that block trackers and malicious scripts, and use a reputable antivirus and ad-blocker for an extra layer of protection." },
                        { "updates", "Software updates are essential for security because they patch vulnerabilities that attackers exploit. Outdated systems are one of the most common entry points for cybercriminals. Updates often include fixes for newly discovered security flaws, performance improvements, and new protection features. Enable automatic updates whenever possible for your operating system, applications, and antivirus software to ensure you remain protected without needing manual intervention." },
                        { "identity theft", "Identity theft occurs when criminals steal your personal information to commit fraud, such as opening bank accounts or making purchases in your name. Information can be obtained through phishing, data breaches, or social media oversharing. Protect yourself by limiting the personal details you share online, monitoring financial statements regularly, and using credit alerts if available. If identity theft occurs, report it immediately to your bank and relevant authorities to minimize damage." },
                        { "social engineering", "Social engineering attacks manipulate human psychology rather than technical vulnerabilities. Attackers may pretend to be trusted individuals such as bank officials, IT staff, or coworkers to convince you to reveal confidential information or perform unsafe actions. These attacks often rely on urgency, fear, or curiosity to pressure victims into acting quickly. Always verify identities before sharing sensitive information, especially through phone calls or messages. Organizations typically have procedures and will not ask for passwords directly." },
                        { "2fa", "Two-Factor Authentication (2FA) adds an extra layer of security beyond just a password. It requires a second form of verification, such as a code sent to your phone, an authentication app, or biometric data like a fingerprint. Even if a hacker steals your password, they cannot access your account without the second factor. Enabling 2FA on important accounts such as email, banking, and social media significantly reduces the risk of unauthorized access." },
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

                    // Natural-language handler: "what can you advise me about X" — supports multiple topics
                    var adviseMatch = System.Text.RegularExpressions.Regex.Match(lower, "what can (?:you|u) (?:advice|advise) me about(?: the)?\\s+(.+)");
                    if (adviseMatch.Success)
                    {
                        var requested = adviseMatch.Groups[1].Value.Trim();
                        // Split requested into possible multiple topics by commas, 'and', or 'then'
                        var parts = System.Text.RegularExpressions.Regex.Split(requested, "\\s*(?:,|\\band\\b|\\bthen\\b)\\s*");
                        var answers = new System.Collections.Generic.List<string>();
                        foreach (var p in parts)
                        {
                            var token = System.Text.RegularExpressions.Regex.Replace(p.Trim(), "[^a-z0-9 ]+", "");
                            if (string.IsNullOrEmpty(token)) continue;
                            if (topicsCore.ContainsKey(token)) { answers.Add(topicsCore[token]); continue; }
                            // try token match within topic keys (token may be a single word)
                            bool found = false;
                            foreach (var kv in topicsCore)
                            {
                                if (kv.Key.Equals(token, System.StringComparison.OrdinalIgnoreCase) || kv.Key.Contains(token)) { answers.Add(kv.Value); found = true; break; }
                                var keyTokens = kv.Key.Split(' ');
                                foreach (var kt in keyTokens)
                                {
                                    if (kt.Equals(token, System.StringComparison.OrdinalIgnoreCase)) { answers.Add(kv.Value); found = true; break; }
                                }
                                if (found) break;
                            }
                        }
                        if (answers.Count > 0) { foreach (var a in answers) TypeText(Program.CurrentUserName + ", " + a + "\n"); break; }
                        // fallback: list available topics
                        TypeText(Program.CurrentUserName + ", I can advise about: " + string.Join(", ", topicsCore.Keys));
                        break;
                    }



                    var topics = topicsCore;

                    // Tokenize user input to compare individual words
                    var tokens = new System.Collections.Generic.List<string>();
                    foreach (var t in System.Text.RegularExpressions.Regex.Split(lower, "[^a-z0-9]+"))
                    {
                        if (!string.IsNullOrEmpty(t)) tokens.Add(t);
                    }

                    var matched = new System.Collections.Generic.List<string>();

                    // Exact contains checks first (handles multi-word phrases too)
                    // We prefer exact phrase matches to preserve user intent
                    // (for example, the phrase "mobile security" should match only that topic).
                    foreach (var kv in topics)
                    {
                        if (lower.Contains(kv.Key))
                        {
                            matched.Add(kv.Key);
                        }
                        else
                        {
                            // check if any token equals a token from the key
                            var keyTokens = kv.Key.Split(' ');
                            foreach (var kt in keyTokens)
                            {
                                foreach (var it in tokens)
                                {
                                    if (it == kt)
                                    {
                                        matched.Add(kv.Key);
                                        break;
                                    }
                                }
                                if (matched.Contains(kv.Key)) break;
                            }
                        }
                    }

                    if (matched.Count > 0)
                    {
                        // answer all matched topics
                        foreach (var key in matched)
                        {
                            TypeText($"{Program.CurrentUserName}, {topics[key]}");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                    // No exact match — attempt fuzzy suggestions
                        // Build a list of suggested topics using Levenshtein distance
                        // to catch common misspellings (e.g. 'phising' -> 'phishing').
                        var suggestions = new System.Collections.Generic.List<string>();
                        foreach (var kv in topics)
                        {
                            var keyTokens = kv.Key.Split(' ');
                            foreach (var kt in keyTokens)
                            {
                                foreach (var it in tokens)
                                {
                                    int dist = LevenshteinDistance(kt, it);
                                    int threshold = Math.Max(1, kt.Length / 3);
                                    if (dist <= threshold)
                                    {
                                        if (!suggestions.Contains(kv.Key)) suggestions.Add(kv.Key);
                                    }
                                }
                            }
                        }

                        if (suggestions.Count > 0)
                        {
                            // Ask user to confirm each suggestion in order
                            bool handled = false;
                            foreach (var s in suggestions)
                            {
                                TypeText($"I think you meant \"{s}\". Please type 'yes' or 'no':");
                                Console.Write("Your choice: ");
                                string reply = Console.ReadLine()?.Trim().ToLower();
                                if (!string.IsNullOrEmpty(reply) && (reply.StartsWith("y") || reply == "yes"))
                                {
                                    TypeText($"{Program.CurrentUserName}, {topics[s]}");
                                    Console.WriteLine();
                                    handled = true;
                                    break;
                                }
                                // if user says no, try next suggestion
                            }

                            if (!handled)
                            {
                                // If none of the fuzzy suggestions were accepted, show a polite fallback.
                                TypeText("Sorry, I can't answer that at the moment. I am still updating and will provide that level of information soon.");
                            }
                        }
                        else
                        {
                            TypeText("Sorry, I can't answer that at the moment. I am still updating and will provide that level of information soon.");
                        }
                    }

                    break;
            }
        }
    }

    // =========================================================
    // 📋 MENU UI
    // =========================================================
    // Print the main menu to the console.
    static void DisplayMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;

        // Display the primary user menu with numbered options
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

    // =========================================================
    // ❌ ERROR DISPLAY
    // =========================================================
    // Print an error message in red.
    static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    // =========================================================
    // ✨ TYPING ANIMATION
    // =========================================================
    // Type out a string slowly. Restores previous console color after.
    static void TypeText(string text)
    {
        // Print text character-by-character to simulate typing
        var previousColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan; // ensure typed responses appear in cyan
            foreach (char c in text)
            {
                Console.Write(c);
                // Small delay to emulate human typing speed for UX polish
                Thread.Sleep(18);
            }
            Console.WriteLine();
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }

    // Variant of TypeText that allows temporarily specifying a color
    // Type out a string slowly using a specific color for the text.
    static void TypeTextColor(string text, ConsoleColor color)
    {
        var prev = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(18);
            }
            Console.WriteLine();
        }
        finally
        {
            Console.ForegroundColor = prev;
        }
    }

    // Levenshtein distance helper to detect misspellings
    // This is used by fuzzy-suggestion logic to suggest corrections
    // Compute Levenshtein distance between two strings (used for fuzzy match).
    static int LevenshteinDistance(string a, string b)
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

    // =========================================================
    // 👋 EXIT MESSAGE
    // =========================================================
    // Show a friendly exit message and end the program.
    public static void ExitProgram(string name)
    {
        // Friendly exit sequence with multiple informative lines
        Console.ForegroundColor = ConsoleColor.Cyan;

        TypeText($"\nGoodbye, {name}.");
        TypeText("Thank you for using Cyber Times With Abo.");
        TypeText("Stay vigilant and stay safe online.");
        TypeText("Remember, cybersecurity is a shared responsibility. Together, we can create a safer digital world for everyone.");
        TypeText("Made by Abongile Manqana");

        // Reset console color state before returning control to the OS
        Console.ResetColor();
    }
}